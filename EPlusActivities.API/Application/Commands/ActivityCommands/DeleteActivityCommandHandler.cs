using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteActivityCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteActivityCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IActivityActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(ActivityActor)
                )
                .DeleteActivity(command);
            return Unit.Value;
        }
    }
}
