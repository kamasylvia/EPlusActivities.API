using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.UserActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class CreateAdminOrManagerCommandHandler : IRequestHandler<CreateAdminOrManagerCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CreateAdminOrManagerCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            CreateAdminOrManagerCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IUserActor>(new ActorId(command.UserName), nameof(UserActor))
                .CreateAdminOrManager(command);
            return Unit.Value;
        }
    }
}
