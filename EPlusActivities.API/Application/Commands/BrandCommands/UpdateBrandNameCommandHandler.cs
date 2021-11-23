using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.BrandActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class UpdateBrandNameCommandHandler
        :
          IRequestHandler<UpdateBrandNameCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateBrandNameCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdateBrandNameCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IBrandActor>(
                    new ActorId(
                        command.Id.ToString()
                    ),
                    nameof(BrandActor)
                )
                .UpdateBrandName(command);
            return Unit.Value;
        }
    }
}
