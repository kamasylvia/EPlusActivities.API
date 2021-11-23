using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.LotteryCommands;
using EPlusActivities.API.Dtos.LotteryDtos;

namespace EPlusActivities.API.Application.Actors.LotteryActors
{
    public interface ILotteryActor : IActor
    {
        Task<IEnumerable<LotteryDto>> Draw(DrawCommand command);
        Task UpdateLotteryRecord(UpdateLotteryRecordCommand command);
        Task DeleteLotteryRecord(DeleteLotteryRecordCommand command);
    }
}
