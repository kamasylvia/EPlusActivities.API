using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.UserActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public DeleteUserCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            DeleteUserCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IUserActor>(new ActorId(command.Id.ToString()), nameof(UserActor))
                .DeleteUser(command);
            return Unit.Value;
        }
    }
}
