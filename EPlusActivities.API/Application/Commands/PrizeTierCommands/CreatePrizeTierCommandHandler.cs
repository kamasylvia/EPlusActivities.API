using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.PrizeTierActors;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class CreatePrizeTierCommandHandler
        : IRequestHandler<CreatePrizeTierCommand, PrizeTierDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreatePrizeTierCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<PrizeTierDto> Handle(
            CreatePrizeTierCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IPrizeTierActor>(
                    new ActorId(command.ActivityId + command.Name),
                    nameof(PrizeTierActor)
                )
                .CreatePrizeTier(command);
    }
}
