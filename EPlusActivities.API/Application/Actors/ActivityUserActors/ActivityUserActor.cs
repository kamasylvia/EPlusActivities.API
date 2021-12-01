using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.ActivityUserActors
{
    public class ActivityUserActor : Actor, IActivityUserActor
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityService _activityService;
        private readonly IGeneralLotteryRecordsRepository _statementRepository;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGeneratorService;

        public ActivityUserActor(
            ActorHost host,
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        ) : base(host)
        {
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository = activityUserRepository;
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _statementRepository =
                statementRepository ?? throw new ArgumentNullException(nameof(statementRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
        }

        public async Task BindActivityAndUser(BindActivityAndUserCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.Value.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(command.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId.Value,
                command.UserId.Value
            );
            if (activityUser is not null)
            {
                throw new ConflictException("The user had already joined the activity.");
            }
            #endregion

            #region Create an ActivityUser link
            activityUser = new ActivityUser { Activity = activity, User = user, };
            #endregion

            #region Database operations
            await _activityUserRepository.AddAsync(activityUser);
            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException();
            }
            #endregion
        }

        public async Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdQuery request
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            return _mapper.Map<IEnumerable<ActivityUserDto>>(
                await _activityService.GetAvailableActivityUserLinksAsync(
                    request.UserId.Value,
                    request.AvailableChannel,
                    request.StartTime,
                    request.EndTime
                )
            );
        }

        public async Task<IEnumerable<ActivityUserDto>> BindUserWithAvailableActivitiesAsync(
            BindAvailableActivitiesCommand command
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            return _mapper.Map<IEnumerable<ActivityUserDto>>(
                await _activityService.BindUserWithAvailableActivities(
                    command.UserId.Value,
                    command.ChannelCode
                )
            );
        }

        public async Task<ActivityUserForRedeemDrawsResponseDto> Redeem(RedeemCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(command.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var generalLotteryRecords = await _statementRepository.FindByDateAsync(
                command.ActivityId.Value,
                command.Channel,
                DateTime.Today
            );
            var requireNewStatement = generalLotteryRecords is null;
            if (requireNewStatement)
            {
                generalLotteryRecords = new GeneralLotteryRecords
                {
                    Activity = activity,
                    DateTime = DateTime.Today,
                    Channel = command.Channel,
                };
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId.Value,
                command.UserId.Value
            );
            if (activityUser is null)
            {
                throw new NotFoundException("Could not find the ActivityUser link.");
            }

            // 今日没登陆过的用户，每日兑换次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);

            // 超过每日兑换限制
            if (!(activityUser.TodayUsedRedempion + command.Count > activity.DailyRedemptionLimit))
            {
                activityUser.TodayUsedRedempion += command.Count;
            }
            else
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the daily maximum number of redemption of this activity."
                );
            }

            var cost = activity.RequiredCreditForRedeeming * command.Count;
            if (cost > user.Credit)
            {
                throw new BadRequestException("The user did not have enough credits.");
            }
            #endregion

            #region Connect member server
            var member = await _memberService.GetMemberAsync(user.PhoneNumber, command.Channel);
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = member.Body.Content.MemberId,
                points = cost ?? 0,
                reason = command.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Subtraction
            };
            var memberForUpdateCreditResponseDto = await _memberService.UpdateCreditAsync(
                userId: command.UserId.Value,
                channelCode: command.Channel,
                requestDto: memberForUpdateCreditRequestDto
            );
            #endregion

            #region Update the user's credit
            user.Credit = memberForUpdateCreditResponseDto.Body.Content.NewPoints;
            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                throw new DatabaseUpdateException(updateUserResult.ToString());
            }
            #endregion

            #region Update ActivityUser link
            activityUser.RemainingDraws += command.Count;
            generalLotteryRecords.Redemption += command.Count;

            _activityUserRepository.Update(activityUser);
            if (requireNewStatement)
            {
                await _statementRepository.AddAsync(generalLotteryRecords);
            }
            else
            {
                _statementRepository.Update(generalLotteryRecords);
            }

            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }
            #endregion

            return _mapper.Map<ActivityUserForRedeemDrawsResponseDto>(activityUser);
        }
    }
}
