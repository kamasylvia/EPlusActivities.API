using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Utils;
using Microsoft.Extensions.Configuration;

namespace EPlusActivities.API.Services.LotteryService
{
    public class LotteryService : ILotteryService
    {
        private readonly IConfiguration _configuration;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IMapper _mapper;

        public LotteryService(
            IConfiguration configuration,
            IPrizeItemRepository prizeItemRepository,
            IMapper mapper
        ) {
            _configuration = configuration;
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LotteryRecordsForManagerResponse> CreateLotteryForDownload(
            IEnumerable<Lottery> lotteries
        ) {
            var response = new List<LotteryRecordsForManagerResponse>();
            lotteries.ToList()
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
                            default:
                                break;
                        }
                        var responseItem = _mapper.Map<LotteryRecordsForManagerResponse>(item);
                        responseItem.DateTime = item.DateTime;
                        responseItem.PrizeContent = prizeContent;
                        response.Add(responseItem);
                    }
                );
            return response.OrderBy(x => x.DateTime);
        }

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        public (MemoryStream, string) DownloadLotteryRecords(
            IEnumerable<GeneralLotteryRecords> generals,
            IEnumerable<Lottery> details
        ) {
            var memoryStream = new MemoryStream();
            var contentType = string.Empty;
            using (
                var spreadsheetDocument = SpreadsheetDocument.CreateFromTemplate(
                    _configuration["LotteryDataExcelTemplatePath"]
                )
            ) {
                FillGeneralSheet(spreadsheetDocument, generals);
                FillDetailSheet(spreadsheetDocument, details);
                spreadsheetDocument.Clone(memoryStream);
                contentType = spreadsheetDocument.RootPart.ContentType;
            }

            memoryStream.Position = 0;
            return (memoryStream, contentType);
        }

        private void FillGeneralSheet(
            SpreadsheetDocument spreadsheetDocument,
            IEnumerable<GeneralLotteryRecords> generals
        ) {
            var list = generals.ToList();
            var workbookPart = spreadsheetDocument.WorkbookPart;
            var worksheetPart = workbookPart.WorksheetParts.First();

            for (uint i = 0; i < list.Count; i++)
            {
                // 日期
                var cellA = OpenXmlUtils.InsertCellInWorksheet("A", i + 3, worksheetPart);
                cellA.CellValue = new CellValue(list[Convert.ToInt32(i)].DateTime.Date);
                cellA.DataType = new EnumValue<CellValues>(CellValues.Date);

                // 抽奖次数
                var cellB = OpenXmlUtils.InsertCellInWorksheet("B", i + 3, worksheetPart);
                cellB.CellValue = new CellValue(list[Convert.ToInt32(i)].Draws);
                cellB.DataType = new EnumValue<CellValues>(CellValues.Number);

                // 中奖次数
                var cellC = OpenXmlUtils.InsertCellInWorksheet("B", i + 3, worksheetPart);
                cellC.CellValue = new CellValue(list[Convert.ToInt32(i)].Winners);
                cellC.DataType = new EnumValue<CellValues>(CellValues.Number);

                // 兑换次数
                var cellD = OpenXmlUtils.InsertCellInWorksheet("D", i + 3, worksheetPart);
                cellD.CellValue = new CellValue(list[Convert.ToInt32(i)].Redemption);
                cellD.DataType = new EnumValue<CellValues>(CellValues.Number);
            }

            // 总计
            var cellTotalA = OpenXmlUtils.InsertCellInWorksheet(
                "A",
                Convert.ToUInt32(list.Count + 3),
                worksheetPart
            );
            cellTotalA.CellValue = new CellValue("总计");
            cellTotalA.DataType = new EnumValue<CellValues>(CellValues.String);

            // 总抽奖次数
            var cellTotalB = OpenXmlUtils.InsertCellInWorksheet(
                "B",
                Convert.ToUInt32(list.Count + 3),
                worksheetPart
            );
            cellTotalB.CellValue = new CellValue(list.Sum(item => item.Draws));
            cellTotalB.DataType = new EnumValue<CellValues>(CellValues.Number);

            // 总中奖次数
            var cellTotalC = OpenXmlUtils.InsertCellInWorksheet(
                "C",
                Convert.ToUInt32(list.Count + 3),
                worksheetPart
            );
            cellTotalC.CellValue = new CellValue(list.Sum(item => item.Winners));
            cellTotalC.DataType = new EnumValue<CellValues>(CellValues.Number);

            // 总兑换次数
            var cellTotalD = OpenXmlUtils.InsertCellInWorksheet(
                "D",
                Convert.ToUInt32(list.Count + 3),
                worksheetPart
            );
            cellTotalD.CellValue = new CellValue(list.Sum(item => item.Redemption));
            cellTotalD.DataType = new EnumValue<CellValues>(CellValues.Number);

            worksheetPart.Worksheet.Save();
        }

        private void FillDetailSheet(
            SpreadsheetDocument spreadsheetDocument,
            IEnumerable<Lottery> details
        ) {
            var data = CreateLotteryForDownload(details).ToList();

            var workbookPart = spreadsheetDocument.WorkbookPart;
            var worksheetPart = workbookPart.WorksheetParts.LastOrDefault();

            for (uint i = 0; i < data.Count; i++)
            {
                // 日期
                var cellA = OpenXmlUtils.InsertCellInWorksheet("A", i + 3, worksheetPart);
                cellA.CellValue = new CellValue(data[Convert.ToInt32(i)].DateTime.Value.Date);
                cellA.DataType = new EnumValue<CellValues>(CellValues.Date);

                // 时间
                var cellB = OpenXmlUtils.InsertCellInWorksheet("B", i + 3, worksheetPart);
                cellB.CellValue = new CellValue(
                    data[Convert.ToInt32(i)].DateTime.Value.TimeOfDay.ToString()
                );
                cellB.DataType = new EnumValue<CellValues>(CellValues.Date);

                // 用户手机号
                var cellC = OpenXmlUtils.InsertCellInWorksheet("C", i + 3, worksheetPart);
                cellC.CellValue = new CellValue(data[Convert.ToInt32(i)].PhoneNumber);
                cellC.DataType = new EnumValue<CellValues>(CellValues.String);

                // 抽奖渠道
                var cellD = OpenXmlUtils.InsertCellInWorksheet("D", i + 3, worksheetPart);
                cellD.CellValue = new CellValue(data[Convert.ToInt32(i)].ChannelCode);
                cellD.DataType = new EnumValue<CellValues>(CellValues.String);

                // 抽奖活动号
                var cellE = OpenXmlUtils.InsertCellInWorksheet("E", i + 3, worksheetPart);
                cellD.CellValue = new CellValue(data[Convert.ToInt32(i)].ActivityCode);
                cellE.DataType = new EnumValue<CellValues>(CellValues.String);

                // 抽奖活动名称
                var cellF = OpenXmlUtils.InsertCellInWorksheet("F", i + 3, worksheetPart);
                cellF.CellValue = new CellValue(data[Convert.ToInt32(i)].ActivityName);
                cellF.DataType = new EnumValue<CellValues>(CellValues.String);

                // 用户 ID
                var cellG = OpenXmlUtils.InsertCellInWorksheet("F", i + 3, worksheetPart);
                cellG.CellValue = new CellValue(data[Convert.ToInt32(i)].UserId.ToString());
                cellG.DataType = new EnumValue<CellValues>(CellValues.String);

                // 中奖名称
                var cellH = OpenXmlUtils.InsertCellInWorksheet("H", i + 3, worksheetPart);
                cellH.CellValue = new CellValue(data[Convert.ToInt32(i)].PrizeTierName);
                cellH.DataType = new EnumValue<CellValues>(CellValues.String);

                // 奖励类型
                var cellI = OpenXmlUtils.InsertCellInWorksheet("I", i + 3, worksheetPart);
                cellI.CellValue = new CellValue(data[Convert.ToInt32(i)].PrizeType);
                cellI.DataType = new EnumValue<CellValues>(CellValues.String);

                // 中奖内容
                var cellJ = OpenXmlUtils.InsertCellInWorksheet("J", i + 3, worksheetPart);
                cellJ.CellValue = new CellValue(data[Convert.ToInt32(i)].PrizeType);
                cellJ.DataType = new EnumValue<CellValues>(CellValues.String);

                // 消耗积分
                var cellK = OpenXmlUtils.InsertCellInWorksheet("K", i + 3, worksheetPart);
                cellK.CellValue = new CellValue(data[Convert.ToInt32(i)].UsedCredit);
                cellK.DataType = new EnumValue<CellValues>(CellValues.Number);
            }

            // 总计
            var cellTotalA = OpenXmlUtils.InsertCellInWorksheet(
                "J",
                Convert.ToUInt32(data.Count + 3),
                worksheetPart
            );
            cellTotalA.CellValue = new CellValue("总计");
            cellTotalA.DataType = new EnumValue<CellValues>(CellValues.String);

            // 总抽奖次数
            var cellTotalB = OpenXmlUtils.InsertCellInWorksheet(
                "K",
                Convert.ToUInt32(data.Count + 3),
                worksheetPart
            );
            cellTotalB.CellValue = new CellValue(data.Sum(item => item.UsedCredit));
            cellTotalB.DataType = new EnumValue<CellValues>(CellValues.Number);

            worksheetPart.Worksheet.Save();
        }

        /// <summary>
        /// 抽奖方法：
        ///     PrizeTier：几等奖。这里是按一档多奖品来写的，一档一奖也可以用，档内随机概率自动上升至 100%。
        ///     PrizeItem：奖品。
        /// </summary>
        /// <param name="activity">抽奖活动</param>
        /// <returns>(几等奖, 奖品)</returns>
        public async Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity)
        {
            var total = 0;
            var random = new Random();
            var flag = random.Next(100);
            var prizeTiers = activity.PrizeTiers;

            PrizeTier prizeTier = null;
            foreach (var item in prizeTiers)
            {
                // 如果上次中奖日期早于今天，今日中奖人数清零
                if (item.LastDate < DateTime.Now.Date)
                    item.TodayWinnerCount = 0;

                total += item.Percentage;
                if (total > flag && !(item.TodayWinnerCount >= item.DailyLimit))
                {
                    prizeTier = item;

                    // 提取该档包含多奖品列表
                    var prizeItems = (
                        await _prizeItemRepository.FindByPrizeTierIdAsync(prizeTier.Id.Value)
                    ).Where(item => !(item.Stock <= 0));

                    // 如果该档奖品全部没有库存，顺延到下一档
                    if (prizeItems.Count() <= 0)
                        continue;

                    // 在奖品档次中包含的奖品中抽选一件奖品
                    var prizeItem = prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count()));
                    item.TodayWinnerCount++;

                    // 更新上次中奖日期
                    if (item.LastDate < DateTime.Now.Date)
                        item.LastDate = DateTime.Now.Date;

                    // 总库存减一
                    prizeItem.Stock--;

                    return (
                        prizeTier,
                        prizeItems.ElementAtOrDefault(random.Next(prizeItems.Count()))
                    );
                }
            }

            return (null, null);
        }
    }
}
