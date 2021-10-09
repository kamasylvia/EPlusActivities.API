using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IStatementRepository : IRepository<Statement>
    {
        Task<IEnumerable<Statement>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        );

        Task<Statement> FindByDateAsync(Guid activityId, ChannelCode channel, DateTime date);
    }
}
