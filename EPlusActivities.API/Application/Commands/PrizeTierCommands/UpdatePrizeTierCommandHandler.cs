using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.PrizeTierActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class UpdatePrizeTierCommandHandler : IRequestHandler<UpdatePrizeTierCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdatePrizeTierCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdatePrizeTierCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IPrizeTierActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(PrizeTierActor)
                )
                .UpdatePrizeTier(command);
            return Unit.Value;
        }
    }
}
