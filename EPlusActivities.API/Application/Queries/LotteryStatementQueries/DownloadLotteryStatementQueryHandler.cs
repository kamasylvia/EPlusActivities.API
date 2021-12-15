using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Services.LotteryStatementService;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class DownloadLotteryStatementQueryHandler
        : IRequestHandler<DownloadLotteryStatementQuery, (MemoryStream, string)>
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

        public async Task<(MemoryStream, string)> Handle(
            DownloadLotteryStatementQuery request,
            CancellationToken cancellationToken
        ) =>
        await _lotteryStatementService.DownloadLotterStatementAsync(request);
    }
}
