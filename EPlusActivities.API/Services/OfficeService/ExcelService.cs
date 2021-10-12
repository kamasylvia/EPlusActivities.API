using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;

namespace EPlusActivities.API.Services.OfficeService
{
    public class ExcelService : IExcelService
    {
        public Task<byte[]> DownloadLotteryResults(
            IEnumerable<LotteryRecordsForManagerResponse> lotteries
        ) {
            throw new NotImplementedException();
        }
    }
}
