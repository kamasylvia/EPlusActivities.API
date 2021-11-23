using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.FileActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UploadFileCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UploadFileCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IFileActor>(
                    new ActorId(command.OwnerId + command.Key),
                    nameof(FileActor)
                )
                .UploadFile(command);
            return Unit.Value;
        }
    }
}
