using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.LotteryService
{
    public interface ILotteryService
    {
        Task<(PrizeTier, PrizeItem)> DrawPrizeAsync(Activity activity);

        IEnumerable<DetailedLotteryStatementResponse> CreateLotteryForDownload(
            IEnumerable<Lottery> lotteries
        );

        (MemoryStream, string) DownloadLotteryRecords(
            IEnumerable<GeneralLotteryRecords> generals,
            IEnumerable<Lottery> details
        );
    }
}
