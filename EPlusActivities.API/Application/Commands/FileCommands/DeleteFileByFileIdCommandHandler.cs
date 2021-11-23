using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.FileActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.FileCommands
{
    public class DeleteFileByFileIdCommandHandler
        :
          IRequestHandler<DeleteFileByFileIdCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteFileByFileIdCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteFileByFileIdCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                           .CreateActorProxy<IFileActor>(
                               new ActorId(
                                   command.FileId.ToString()
                               ),
                               nameof(FileActor)
                           )
                           .DeleteFileByFileId(command);
            return Unit.Value;
        }
    }
}
