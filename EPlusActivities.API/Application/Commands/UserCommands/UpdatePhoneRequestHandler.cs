using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.UserActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class UpdatePhoneCommandHandler : IRequestHandler<UpdatePhoneCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public UpdatePhoneCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<Unit> Handle(
            UpdatePhoneCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                .CreateActorProxy<IUserActor>(new ActorId(command.Id.ToString()), nameof(UserActor))
                .UpdatePhone(command);
            return Unit.Value;
        }
    }
}
