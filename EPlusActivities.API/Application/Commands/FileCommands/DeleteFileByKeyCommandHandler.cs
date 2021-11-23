using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.FileActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DeleteFileByKeyCommandHandler : IRequestHandler<DeleteFileByKeyCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteFileByKeyCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteFileByKeyCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IFileActor>(
                    new ActorId(command.OwnerId + command.Key),
                    nameof(FileActor)
                )
                .DeleteFileByKey(command);
            return Unit.Value;
        }
    }
}
