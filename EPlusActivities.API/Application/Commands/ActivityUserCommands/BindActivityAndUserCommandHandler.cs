using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class BindActivityAndUserCommandHandler
        :
          IRequestHandler<BindActivityAndUserCommand>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public BindActivityAndUserCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task<Unit> Handle(
            BindActivityAndUserCommand command,
            CancellationToken cancellationToken
        )
        {
            await _actorProxyFactory
                            .CreateActorProxy<IActivityUserActor>(
                                new ActorId(
                                    command.ActivityId.Value.ToString() + command.UserId.Value.ToString()
                                ),
                                nameof(ActivityUserActor)
                            )
                            .BindActivityAndUser(command);
            return Unit.Value;
        }
    }
}
