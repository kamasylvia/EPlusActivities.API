using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.BrandActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteBrandCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteBrandCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IBrandActor>(
                    new ActorId(command.Id.ToString()),
                    nameof(BrandActor)
                )
                .DeleteBrand(command);
            return Unit.Value;
        }
    }
}
