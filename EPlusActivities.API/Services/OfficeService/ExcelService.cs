using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Services.OfficeService
{
    [Service(ServiceLifetime.Scoped)]
    public class ExcelService : IExcelService
    {
        public Task<byte[]> DownloadLotteryResults(
            IEnumerable<LotteryRecordsForManagerResponse> lotteries
        )
        {
            throw new NotImplementedException();
        }
    }
}
