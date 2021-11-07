using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class RedeemCommandHandler
        : BaseCommandHandler,
          IRequestHandler<RedeemCommand, ActivityUserForRedeemDrawsResponseDto>
    {
        public RedeemCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            IMapper mapper,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository statementRepository
        )
            : base(
                activityRepository,
                memberService,
                userManager,
                activityUserRepository,
                mapper,
                idGeneratorService,
                activityService,
                statementRepository
            ) { }

        public async Task<ActivityUserForRedeemDrawsResponseDto> Handle(
            RedeemCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var generalLotteryRecords = await _statementRepository.FindByDateAsync(
                request.ActivityId.Value,
                request.Channel,
                DateTime.Today
            );
            var requireNewStatement = generalLotteryRecords is null;
            if (requireNewStatement)
            {
                generalLotteryRecords = new GeneralLotteryRecords
                {
                    Activity = activity,
                    DateTime = DateTime.Today,
                    Channel = request.Channel,
                };
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
            );
            if (activityUser is null)
            {
                throw new NotFoundException("Could not find the ActivityUser link.");
            }

            // 今日没登陆过的用户，每日兑换次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);

            // 超过每日兑换限制
            if (!(activityUser.TodayUsedRedempion + request.Count <= activity.DailyRedemptionLimit))
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the daily maximum number of redemption of this activity."
                );
            }
            else
            {
                activityUser.TodayUsedRedempion += request.Count;
            }

            var cost = activity.RequiredCreditForRedeeming * request.Count;
            if (cost > user.Credit)
            {
                throw new BadRequestException("The user did not have enough credits.");
            }
            #endregion

            #region Connect member server
            var member = await _memberService.GetMemberAsync(user.PhoneNumber, request.Channel);
            var memberForUpdateCreditRequestDto = new MemberForUpdateCreditRequestDto
            {
                memberId = member.Body.Content.MemberId,
                points = cost ?? 0,
                reason = request.Reason,
                sheetId = _idGeneratorService.NextId().ToString(),
                updateType = CreditUpdateType.Subtraction
            };
            var memberForUpdateCreditResponseDto = await _memberService.UpdateCreditAsync(
                userId: request.UserId.Value,
                channelCode: request.Channel,
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
            activityUser.RemainingDraws += request.Count;
            generalLotteryRecords.Redemption += request.Count;

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
