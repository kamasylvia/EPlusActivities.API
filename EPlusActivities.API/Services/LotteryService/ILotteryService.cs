using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.LotteryService
{
    public interface ILotteryService
    {
        Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity);

        IEnumerable<GetLotteryDetailsResponse> CreateLotteryForDownload(
            IEnumerable<LotteryDetail> lotteries
        );

        (MemoryStream, string) DownloadLotteryRecords(
            IEnumerable<LotterySummary> generals,
            IEnumerable<LotteryDetail> details
        );
    }
}
