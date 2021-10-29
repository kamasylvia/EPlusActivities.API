using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryDtos;
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
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DrawCommandHandler
        : BaseCommandHandler,
          IRequestHandler<DrawCommand, IEnumerable<LotteryDto>>
    {
        public DrawCommandHandler(
            ILotteryRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            IRepository<Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            IGeneralLotteryRecordsRepository generalLotteryRecordsRepository,
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
                generalLotteryRecordsRepository,
                activityService
            ) { }

        public async Task<IEnumerable<LotteryDto>> Handle(
            DrawCommand request,
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

            if (DateTime.Today < activity.StartTime || DateTime.Today > activity.EndTime)
            {
                throw new BadRequestException("This activity is expired.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
            );

            if (activityUser is null)
            {
                throw new BadRequestException("The user had to join the activity first.");
            }

            // 剩余抽奖次数不足
            if (activityUser.RemainingDraws < request.Count)
            {
                throw new BadRequestException(
                    "The user did not have enough chance to draw a lottery ."
                );
            }

            // 超过全活动周期抽奖次数限制
            if (activityUser.UsedDraws + request.Count > activity.Limit)
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the maximum number of draws of this activity."
                );
            }

            // 今天没登陆过的用户，每日已用抽奖次数清零
            _activityService.UpdateDailyLimitsAsync(user, activityUser);

            // 超过每日抽奖次数限制
            if (activityUser.TodayUsedDraws + request.Count > activity.DailyDrawLimit)
            {
                throw new BadRequestException(
                    "Sorry, the user had already achieved the daily maximum number of draws of this activity."
                );
            }
            // var channel = Enum.Parse<ChannelCode>(request.ChannelCode, true);
            var channel = request.ChannelCode;
            var generalRecords = await _generalLotteryRecordsRepository.FindByDateAsync(
                request.ActivityId.Value,
                channel,
                DateTime.Today
            );
            var requireNewStatement = generalRecords is null;
            if (requireNewStatement)
            {
                generalRecords = new GeneralLotteryRecords
                {
                    Activity = activity,
                    DateTime = DateTime.Today,
                    Channel = channel,
                };
            }
            #endregion

            #region Consume the draws
            activityUser.RemainingDraws -= request.Count;
            activityUser.TodayUsedDraws += request.Count;
            activityUser.UsedDraws += request.Count;
            generalRecords.Draws += request.Count;
            #endregion

            #region Generate the lottery result
            var response = new List<LotteryDto>();

            for (int i = 0; i < request.Count; i++)
            {
                var lottery = _mapper.Map<Lottery>(request);
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
                            user.Credit += lottery.PrizeItem.Credit.Value;
                            if (user.Credit != updateCreditResponseDto.Body.Content.NewPoints)
                            {
                                throw new RemoteServiceException(
                                    "Local credits did not equal to the points on Member Service."
                                );
                            }
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

                            await coupons
                                .ToAsyncEnumerable()
                                .ForEachAwaitAsync(
                                    async item => await _couponRepository.AddAsync(item)
                                );
                            break;
                        default:
                            break;
                    }
                }

                await _lotteryRepository.AddAsync(lottery);
                var result = _mapper.Map<LotteryDto>(lottery);
                result.DateTime = lottery.DateTime; // Skip auto mapper.
                response.Add(result);
            }
            #endregion

            #region Database operations
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (requireNewStatement)
                await _generalLotteryRecordsRepository.AddAsync(generalRecords);
            else
                _generalLotteryRecordsRepository.Update(generalRecords);

            if (!await _lotteryRepository.SaveAsync())
            {
                throw new DatabaseUpdateException("Update database exception");
            }

            if (!userUpdateResult.Succeeded)
            {
                throw new DatabaseUpdateException(userUpdateResult.ToString());
            }
            #endregion

            return response;
        }
    }
}
