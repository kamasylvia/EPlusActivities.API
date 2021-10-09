using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class StatementRepository : RepositoryBase<Statement>, IStatementRepository
    {
        public StatementRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Statement> FindByDateAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime date
        ) =>
            await _context.Statements.Include(s => s.Activity)
                .Where(
                    s =>
                        s.Activity.Id == activityId
                        && s.Channel == channel
                        && s.DateTime.Date == date.Date
                )
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Statement>> FindByDateRangeAsync(
            Guid activityId,
            ChannelCode channel,
            DateTime? startTime,
            DateTime? endTime
        ) =>
            await _context.Statements.Include(s => s.Activity)
                .Where(
                    s =>
                        s.Activity.Id == activityId
                        && s.Channel == channel
                        && !(startTime > s.DateTime)
                        && !(s.DateTime > endTime)
                )
                .ToListAsync();
    }
}
