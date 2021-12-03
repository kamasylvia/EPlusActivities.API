using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Actors.UserActors;
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
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Application.Actors.ActivityUserActors
{
    public class ActivityUserActor : Actor, IActivityUserActor
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IActivityRepository _activityRepository;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivityUserActor> _logger;
        private readonly IGeneralLotteryRecordsRepository _generalLotteryRecordsRepository;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGeneratorService;

        public ActivityUserActor(
            ActorHost host, IActorProxyFactory actorProxyFactory,
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            ILogger<ActivityUserActor> logger,
            IGeneralLotteryRecordsRepository statementRepository
        ) : base(host)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository = activityUserRepository;
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _generalLotteryRecordsRepository =
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
#if DEBUG
            System.Console.WriteLine("进入兑换方法");
#endif

            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(command.ActivityId);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

#if DEBUG
            System.Console.WriteLine("断点 1");
#endif

            var generalLotteryRecords = await _generalLotteryRecordsRepository.FindByDateAsync(
                command.ActivityId.Value,
                command.Channel,
                DateTime.Today
            );

#if DEBUG
            System.Console.WriteLine("断点 2");
#endif
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
#if DEBUG
            System.Console.WriteLine("断点 3");
#endif

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId,
                command.UserId
            );
            if (activityUser is null)
            {
                throw new NotFoundException("Could not find the ActivityUser link.");
            }
#if DEBUG
            System.Console.WriteLine("断点 4");
#endif

            // 今日没登陆过的用户，每日兑换次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);
#if DEBUG
            System.Console.WriteLine("断点 5");
#endif

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
#if DEBUG
            System.Console.WriteLine("断点 6");
#endif

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
#if DEBUG
            System.Console.WriteLine("断点 7");
#endif

            #region Update the user's credit
            user.Credit = memberForUpdateCreditResponseDto.Body.Content.NewPoints;

#if DEBUG
            System.Console.WriteLine("Before _userManager.UpdateAsync");
#endif

            // await _actorProxyFactory
            //     .CreateActorProxy<IUserActor>(new ActorId(user.Id.ToString()), nameof(UserActor))
            //     .UpdateAsync(user);

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                throw new DatabaseUpdateException(updateUserResult.ToString());
            }
            #endregion

#if DEBUG
            System.Console.WriteLine("After _userManager.UpdateAsync");
#endif

            #region Update ActivityUser link
            activityUser.RemainingDraws += command.Count;
            generalLotteryRecords.Redemption += command.Count;
#if DEBUG
            System.Console.WriteLine("断点 9");
#endif

            _activityUserRepository.Update(activityUser);
#if DEBUG
            System.Console.WriteLine("断点 10");
#endif
            if (requireNewStatement)
            {
                await _generalLotteryRecordsRepository.AddAsync(generalLotteryRecords);
#if DEBUG
                System.Console.WriteLine("断点 11");
#endif
            }
            else
            {
                _generalLotteryRecordsRepository.Update(generalLotteryRecords);
#if DEBUG
                System.Console.WriteLine("断点 12");
#endif
            }

            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }
#if DEBUG
            System.Console.WriteLine("断点 13");
#endif
            #endregion

            return _mapper.Map<ActivityUserForRedeemDrawsResponseDto>(activityUser);
        }
    }
}
