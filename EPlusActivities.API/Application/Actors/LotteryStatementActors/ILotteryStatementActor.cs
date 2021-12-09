using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.LotteryStatementCommands;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Application.Actors.LotteryStatementActors
{
    public interface ILotteryStatementActor : IActor
    {
        Task SetReminderAsync();
        Task CreateGeneralLotteryStatementAsync(CreateGeneralLotteryStatementCommand command);
        Task UpdateGeneralLotteryStatementAsync(UpdateGeneralLotteryStatementCommand command);
        // Task GetGeneralLotteryStatement(Guid statementId);

        // Task DeleteGeneralLotteryStatement(Guid statementId);
    }
}
