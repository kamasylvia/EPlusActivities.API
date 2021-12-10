using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Actors.UserActors;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;

namespace EPlusActivities.API.Application.Actors.DrawingActors
{
    public partial class DrawingActor
    {
        public async Task<IEnumerable<DrawingDto>> Draw(DrawCommand command)
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

            var channel = command.ChannelCode;
            var generalRecords = await _lotterySummaryStatementRepository.FindByDateAsync(
                command.ActivityId.Value,
                channel,
                DateTime.Today.ToDateOnly()
            );
            var requireNewStatement = generalRecords is null;
            if (requireNewStatement)
            {
                generalRecords = new LotterySummary
                {
                    Activity = activity,
                    Date = DateTime.Today.ToDateOnly(),
                    Channel = channel,
                };
            }
            #endregion

            #region Consume the draws
            activityUser.RemainingDraws -= command.Count;
            activityUser.TodayUsedDraws += command.Count;
            activityUser.UsedDraws += command.Count;
            generalRecords.Draws += command.Count;
            #endregion

            #region Generate the lottery result
            var response = new List<DrawingDto>();
            var lotteries = new List<LotteryDetail>();

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
                    generalRecords.Winners++;

                    switch (lottery.PrizeItem.PrizeType)
                    {
                        case PrizeType.Credit:
                            var updateCreditResponseDto = await _memberService.UpdateCreditAsync(
                                user.Id,
                                channel,
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
                                channel,
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

                lotteries.Add(lottery);
                var result = _mapper.Map<DrawingDto>(lottery);
                result.DateTime = lottery.DateTime; // Skip auto mapper.
                response.Add(result);
            }
            #endregion

            #region Database operations
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                throw new DatabaseUpdateException(userUpdateResult.ToString());
            }

            if (requireNewStatement)
                await _lotterySummaryStatementRepository.AddAsync(generalRecords);
            else
                _lotterySummaryStatementRepository.Update(generalRecords);

            await _lotteryRepository.AddRangeAsync(lotteries);

            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }
            #endregion

            return response;
        }
    }
}
