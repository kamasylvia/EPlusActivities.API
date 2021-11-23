using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.BrandActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class UpdateBrandCommandHandler
        :
          IRequestHandler<UpdateBrandCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdateBrandCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdateBrandCommand command,
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
