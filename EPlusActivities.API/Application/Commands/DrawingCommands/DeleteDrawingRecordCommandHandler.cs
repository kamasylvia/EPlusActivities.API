using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.DrawingActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public class DeleteDrawingRecordCommandHandler : IRequestHandler<DeleteDrawingRecordCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteDrawingRecordCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task<Unit> Handle(
            DeleteDrawingRecordCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IDrawingActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(DrawingActor)
                )
                .DeleteLotteryRecord(command);
            return Unit.Value;
        }
    }
}
