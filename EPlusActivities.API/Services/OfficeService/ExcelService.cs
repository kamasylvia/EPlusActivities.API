using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Services.OfficeService
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
    public class ExcelService : IExcelService
    {
        public Task<byte[]> DownloadLotteryResults(IEnumerable<GetLotteryDetailsResponse> lotteries)
        {
            throw new NotImplementedException();
        }
    }
}
