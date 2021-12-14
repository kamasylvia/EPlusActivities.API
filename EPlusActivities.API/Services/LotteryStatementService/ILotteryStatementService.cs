using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Application.Queries.LotteryStatementQueries;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.LotteryStatementService
{
    public interface ILotteryStatementService
    {
        Task<XLWorkbook> DownloadLotterStatementAsync(DownloadLotteryStatementQuery request);
        IEnumerable<GetLotteryDetailsResponse> CreateLotteryDetailStatement(
            IEnumerable<LotteryDetail> lotteryDetails
        );
    }
}
