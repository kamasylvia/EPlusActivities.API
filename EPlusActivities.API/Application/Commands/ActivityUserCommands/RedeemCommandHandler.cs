using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Application.Commands.LotteryStatementCommands;
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

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class RedeemCommandHandler
        : ActivityUserRequestHandlerBase,
          IRequestHandler<RedeemCommand, ActivityUserForRedeemDrawsResponseDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IMediator _mediator;

        // public RedeemCommandHandler(IActorProxyFactory actorProxyFactory)
        // {
        //     _actorProxyFactory =
        //         actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        // }

        public RedeemCommandHandler(
            IActivityRepository activityRepository,
            IMemberService memberService,
            Microsoft.AspNetCore.Identity.UserManager<Entities.ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            IMapper mapper,
            IMediator mediator,
            IIdGeneratorService idGeneratorService,
            IActivityService activityService,
            IGeneralLotteryRecordsRepository generalLotteryRecords
        )
            : base(
                activityRepository,
                memberService,
                userManager,
                activityUserRepository,
                mapper,
                idGeneratorService,
                activityService,
                generalLotteryRecords
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ActivityUserForRedeemDrawsResponseDto> Handle(
            RedeemCommand command,
            CancellationToken cancellationToken
        )
        /*
        =>
            await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(
                        command.ActivityId.ToString()
                            + command.UserId.ToString()
                            + command.Channel.ToString()
                    ),
                    nameof(ActivityUserActor)
                )
                .Redeem(command);
        */
        {
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

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId,
                command.UserId
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

            // await _actorProxyFactory
            //     .CreateActorProxy<IUserActor>(new ActorId(user.Id.ToString()), nameof(UserActor))
            //     .UpdateAsync(user);

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                throw new DatabaseUpdateException(updateUserResult.ToString());
            }
            #endregion

            #region Update ActivityUser link
            activityUser.RemainingDraws += command.Count;
            _activityUserRepository.Update(activityUser);
            if (!await _activityUserRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }

            await _mediator.Publish(_mapper.Map<UpdateGeneralLotteryStatementCommand>(command));
            #endregion

            return _mapper.Map<ActivityUserForRedeemDrawsResponseDto>(activityUser);
        }
    }
}
