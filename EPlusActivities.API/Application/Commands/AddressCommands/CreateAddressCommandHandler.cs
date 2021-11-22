using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.AddressActors;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class CreateAddressCommandHandler
        :
          IRequestHandler<CreateAddressCommand, AddressDto>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateAddressCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<AddressDto> Handle(
            CreateAddressCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IAddressActor>(
                    new ActorId(
                        command.UserId.ToString()
                    ),
                    nameof(AddressActor)
                )
                .CreateAddress(command);
    }
}
