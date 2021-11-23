using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.AddressActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteAddressCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteAddressCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IAddressActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(AddressActor)
                )
                .DeleteAddress(command);
            return Unit.Value;
        }
    }
}
