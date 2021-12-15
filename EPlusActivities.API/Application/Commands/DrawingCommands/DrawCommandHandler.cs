using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Commands.LotteryStatementCommands;
using EPlusActivities.API.Dtos.DrawingDtos;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public class DrawCommandHandler
        : DrawingRequestHandlerBase,
          IRequestHandler<DrawCommand, IEnumerable<DrawingDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IMediator _mediator;

        // public DrawCommandHandler(IActorProxyFactory actorProxyFactory)
        // {
        //     _actorProxyFactory =
        //         actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        // }

        public DrawCommandHandler(
            ILotteryDetailRepository lotteryRepository,
            Microsoft.AspNetCore.Identity.UserManager<Entities.ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<Entities.PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IMediator mediator,
            IActivityUserRepository activityUserRepository,
            IRepository<Entities.Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            ILotterySummaryRepository lotterySummaryStatementRepository,
            IActivityService activityService
        )
            : base(
                lotteryRepository,
                userManager,
                activityRepository,
                prizeItemRepository,
                prizeTypeRepository,
                mapper,
                activityUserRepository,
                couponResponseDto,
                lotteryService,
                memberService,
                idGeneratorService,
                lotterySummaryStatementRepository,
                activityService
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DrawingDto>> Handle(
            DrawCommand command,
            CancellationToken cancellationToken
        )
        /*
        =>
            await _actorProxyFactory
                .CreateActorProxy<ILotteryActor>(
                    new ActorId(command.UserId.ToString() + command.ActivityId.ToString()),
                    nameof(LotteryActor)
                )
                .Draw(command);
        */
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

            if (DateTime.Today < activity.StartTime || DateTime.Today > activity.EndTime)
            {
                throw new BadRequestException("This activity is expired.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                command.ActivityId.Value,
                command.UserId.Value
            );

            if (activityUser is null)
            {
                throw new BadRequestException("The user had to join the activity first.");
            }

            // 剩余抽奖次数不足
            if (activityUser.RemainingDraws < command.Count)
            {
                throw new BadRequestException(
                    "The user did not have enough chance to draw a lottery ."
                );
            }

            // 超过全活动周期抽奖次数限制
            if (activityUser.UsedDraws + command.Count > activity.Limit)
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the maximum number of draws of this activity."
                );
            }

            // 今天没登陆过的用户，每日已用抽奖次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);

            // 超过每日抽奖次数限制
            if (activityUser.TodayUsedDraws + command.Count > activity.DailyDrawLimit)
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the daily maximum number of draws of this activity."
                );
            }

            #endregion

            #region Consume the draws
            activityUser.RemainingDraws -= command.Count;
            activityUser.TodayUsedDraws += command.Count;
            activityUser.UsedDraws += command.Count;
            var updateLotterySummaryStatementCommand =
                _mapper.Map<UpdateLotterySummaryStatementCommand>(command);
            await _mediator.Publish(updateLotterySummaryStatementCommand);
            updateLotterySummaryStatementCommand.Draws = 0;
            #endregion

            #region Generate the lottery result
            var response = new List<DrawingDto>();
            var lotteryDetailStatement = new List<LotteryDetail>();

            for (int i = 0; i < command.Count; i++)
            {
                var lottery = _mapper.Map<LotteryDetail>(command);
                lottery.User = user;
                lottery.Activity = activity;
                lottery.DateTime = DateTime.Now;

                (lottery.PrizeTier, lottery.PrizeItem) = await _lotteryService.DrawPrizeAsync(
                    activity
                );

                if (lottery.PrizeTier is not null)
                {
                    lottery.IsLucky = true;
                    updateLotterySummaryStatementCommand.Winners++;

                    switch (lottery.PrizeItem.PrizeType)
                    {
                        case PrizeType.Credit:
                            var updateCreditResponseDto = await _memberService.UpdateCreditAsync(
                                user.Id,
                                command.ChannelCode,
                                new MemberForUpdateCreditRequestDto
                                {
                                    memberId = user.MemberId,
                                    points = lottery.PrizeItem.Credit.Value,
                                    reason = "积分奖品",
                                    sheetId = _idGeneratorService.NextId().ToString(),
                                    updateType = CreditUpdateType.Addition
                                }
                            );
                            user.Credit =
                                updateCreditResponseDto?.Body?.Content?.NewPoints
                                ?? user.Credit + lottery.PrizeItem.Credit.Value;
                            break;
                        case PrizeType.Coupon:
                            var couponResponseDto = await _memberService.ReleaseCouponAsync(
                                command.ChannelCode,
                                new MemberForReleaseCouponRequestDto
                                {
                                    couponActiveCode = lottery.PrizeItem.CouponActiveCode,
                                    memberId = user.MemberId,
                                    qty = 1,
                                    reason = "优惠券奖品"
                                }
                            );
                            var coupons = couponResponseDto?.Body?.Content?
                                .HideCouponCode?.Split(',', StringSplitOptions.TrimEntries)
                                .Select(
                                    code =>
                                        new Coupon
                                        {
                                            User = user,
                                            PrizeItem = lottery.PrizeItem,
                                            Code = code
                                        }
                                );

                            var temp = user.Coupons is null
                                ? new List<Coupon>()
                                : user.Coupons.ToList();
                            temp.AddRange(coupons);
                            user.Coupons = temp;

                            temp = lottery.PrizeItem.Coupons is null
                                ? new List<Coupon>()
                                : lottery.PrizeItem.Coupons.ToList();
                            temp.AddRange(coupons);
                            lottery.PrizeItem.Coupons = temp;

                            await _couponRepository.AddRangeAsync(coupons);
                            break;
                        default:
                            break;
                    }
                }

                lotteryDetailStatement.Add(lottery);
            }
            #endregion

            #region Database operations
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                throw new DatabaseUpdateException(userUpdateResult.ToString());
            }

            await _lotteryRepository.AddRangeAsync(lotteryDetailStatement);

            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }

            await _mediator.Publish(updateLotterySummaryStatementCommand);
            #endregion

            return _mapper.Map<IEnumerable<DrawingDto>>(lotteryDetailStatement);
        }
    }
}
