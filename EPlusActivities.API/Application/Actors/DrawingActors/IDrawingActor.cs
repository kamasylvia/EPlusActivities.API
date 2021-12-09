using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Dtos.LotteryDtos;

namespace EPlusActivities.API.Application.Actors.DrawingActors
{
    public interface IDrawingActor : IActor
    {
        Task<IEnumerable<DrawingDto>> Draw(DrawCommand command);

        Task UpdateLotteryRecord(UpdateDrawingRecordCommand command);

        Task DeleteLotteryRecord(DeleteDrawingRecordCommand command);
    }
}
