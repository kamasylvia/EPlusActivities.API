using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.DrawingActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public class UpdateDrawingRecordCommandHandler
        : INotificationHandler<UpdateDrawingRecordCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateDrawingRecordCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(
            UpdateDrawingRecordCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IDrawingActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(DrawingActor)
                )
                .UpdateLotteryRecord(command);
    }
}
