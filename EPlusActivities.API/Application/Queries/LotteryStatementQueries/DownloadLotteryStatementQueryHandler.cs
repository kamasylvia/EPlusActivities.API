using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EPlusActivities.API.Services.LotteryStatementService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class DownloadLotteryStatementQueryHandler
        : IRequestHandler<DownloadLotteryStatementQuery, XLWorkbook>
    {
        private readonly ILotteryStatementService _lotteryStatementService;

        public DownloadLotteryStatementQueryHandler(
            ILotteryStatementService lotteryStatementService
        )
        {
            _lotteryStatementService =
                lotteryStatementService
                ?? throw new ArgumentNullException(nameof(lotteryStatementService));
        }

        public async Task<XLWorkbook> Handle(
            DownloadLotteryStatementQuery request,
            CancellationToken cancellationToken
        ) => await _lotteryStatementService.DownloadLotterStatementAsync(request);
    }
}
