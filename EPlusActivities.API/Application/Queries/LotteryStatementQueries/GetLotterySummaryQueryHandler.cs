using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
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

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class GetLotterySummaryStatementQueryHandler
        : DrawingRequestHandlerBase,
          IRequestHandler<
              GetLotterySummaryStatementQuery,
              IEnumerable<GetLotterySummaryStatementResponse>
          >
    {
        public GetLotterySummaryStatementQueryHandler(
            ILotteryDetailRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IActivityUserRepository activityUserRepository,
            IRepository<Coupon> couponResponseDto,
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
            ) { }

        public async Task<IEnumerable<GetLotterySummaryStatementResponse>> Handle(
            GetLotterySummaryStatementQuery request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var lotterySummaryStatement =
                await _lotterySummaryStatementRepository.FindByDateRangeAsync(
                    activity.Id.Value,
                    request.Channel,
                    request.StartDate,
                    request.EndDate
                );
            #endregion

            return _mapper.Map<IEnumerable<GetLotterySummaryStatementResponse>>(
                lotterySummaryStatement
            );
        }
    }
}
