using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateActivityCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdateActivityCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IActivityActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(ActivityActor)
                )
                .UpdateActivity(command);
            return Unit.Value;
        }
    }
}
