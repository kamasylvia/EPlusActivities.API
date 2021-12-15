using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Elf.WebAPI.Attributes;
using Elf.WebAPI.Exceptions;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Application.Queries.LotteryStatementQueries;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace EPlusActivities.API.Services.LotteryStatementService
{
    [AutomaticDependencyInjection]
    public class LotteryStatementService : ILotteryStatementService
    {
        private readonly IConfiguration _configuration;
        private readonly ILotterySummaryRepository _lotterySummaryRepository;
        private readonly ILotteryDetailRepository _lotteryDetailRepository;
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;

        public LotteryStatementService(
            IConfiguration configuration,
            ILotterySummaryRepository lotterySummaryRepository,
            ILotteryDetailRepository lotteryDetailRepository,
            IMapper mapper,
            IActivityRepository activityRepository
        )
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _lotterySummaryRepository =
                lotterySummaryRepository
                ?? throw new ArgumentNullException(nameof(lotterySummaryRepository));
            _lotteryDetailRepository =
                lotteryDetailRepository
                ?? throw new ArgumentNullException(nameof(lotteryDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GetLotteryDetailsResponse> CreateLotteryDetailStatement(
            IEnumerable<LotteryDetail> lotteryDetails
        )
        {
            var response = new List<GetLotteryDetailsResponse>();
            lotteryDetails
                .ToList()
                .ForEach(
                    item =>
                    {
                        var prizeContent = string.Empty;
                        switch (item.PrizeItem.PrizeType)
                        {
                            case PrizeType.Coupon:

                                prizeContent = item.PrizeItem.CouponActiveCode;
                                break;
                            case PrizeType.Credit:
                                prizeContent = item.PrizeItem.Credit.ToString();
                                break;
                            case PrizeType.Physical:
                                prizeContent = item.PrizeItem.Name;
                                break;
                            default:
                                break;
                        }
                        var responseItem = _mapper.Map<GetLotteryDetailsResponse>(item);
                        responseItem.PrizeContent = prizeContent;
                        response.Add(responseItem);
                    }
                );

            return response;
        }

        public async Task<(MemoryStream, string)> DownloadLotterStatementAsync(
            DownloadLotteryStatementQuery request
        )
        {
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }

            var lotteryDetails = await _lotteryDetailRepository.FindByDateRangeAsync(
                activity.Id.Value,
                request.Channel,
                request.StartDate.ToDateTime(),
                request.EndDate.ToDateTime()
            );
            var lotterySummaries = await _lotterySummaryRepository.FindByDateRangeAsync(
                activity.Id.Value,
                request.Channel,
                request.StartDate,
                request.EndDate
            );

            #region OpenXML
            var memoryStream = new MemoryStream();
            using var spreadsheetDocument = SpreadsheetDocument.CreateFromTemplate(
                _configuration["LotteryStatementTemplatePath"]
            );
            // FillLotterySummaryStatement();
            // FillLotteryDetailStatement();

            spreadsheetDocument.Clone(memoryStream);
            memoryStream.Position = 0;
            #endregion

            return (memoryStream, spreadsheetDocument.RootPart.ContentType);
        }

        #region OpenXML
        private void FillLotterySummaryStatement(
            Worksheet worksheet,
            IEnumerable<GetLotterySummaryResponse> data
        ) { }

        private void FillLotteryDetailStatement(
            Worksheet worksheet,
            IEnumerable<GetLotteryDetailsResponse> data
        ) { }
        #endregion
    }
}
