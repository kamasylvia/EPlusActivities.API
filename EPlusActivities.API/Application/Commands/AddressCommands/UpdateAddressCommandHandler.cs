using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.AddressActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class UpdateAddressCommandHandler
        :
          IRequestHandler<UpdateAddressCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateAddressCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdateAddressCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
       .CreateActorProxy<IAddressActor>(
           new ActorId(
               command.Id.ToString()
           ),
           nameof(AddressActor)
       )
       .UpdateAddress(command);
            return Unit.Value;
        }
    }
}
