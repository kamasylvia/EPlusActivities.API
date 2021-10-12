using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;

namespace EPlusActivities.API.Services.OfficeService
{
    public interface IExcelService
    {
        Task<Byte[]> DownloadLotteryResults(
            IEnumerable<LotteryRecordsForManagerResponse> lotteries
        );
    }
}
