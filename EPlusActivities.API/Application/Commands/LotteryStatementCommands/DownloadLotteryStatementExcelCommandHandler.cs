using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public class DownloadLotteryStatementExcelCommandHandler
        : DrawingRequestHandlerBase,
          IRequestHandler<DownloadLotteryStatementExcelCommand, FileDto>
    {
        public DownloadLotteryStatementExcelCommandHandler(
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

        public async Task<FileDto> Handle(
            DownloadLotteryStatementExcelCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var lotteries = await activity.LotteryDetailStatement
                .Where(
                    lr =>
                        lr.IsLucky
                        // && Enum.Parse<ChannelCode>(request.Channel, true) == lr.ChannelCode
                        && request.Channel == lr.ChannelCode
                        && !(request.StartDate?.ToDateTime() > lr.DateTime)
                        && !(lr.DateTime > request.EndDate?.ToDateTime())
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion

            var lotterySummaryStatement = await _lotterySummaryStatementRepository.FindByDateRangeAsync(
                activity.Id.Value,
                request.Channel,
                // Enum.Parse<ChannelCode>(request.Channel, true),
                request.StartDate,
                request.EndDate
            );

            var (memoryString, contentType) = _lotteryService.DownloadLotteryRecords(
                lotterySummaryStatement,
                lotteries
            );

            return new FileDto { FileStream = memoryString, ContentType = contentType };
        }
    }
}
