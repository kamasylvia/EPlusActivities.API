using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Application.Queries.UserQueries;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class JoinAvailableActivitiesCommandHandler
        : 
          INotificationHandler<LoginQuery>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public JoinAvailableActivitiesCommandHandler(
            IActorProxyFactory actorProxyFactory
        )
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Handle(LoginQuery notification, CancellationToken cancellationToken) => await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(
                        notification.UserId.ToString() + notification.ChannelCode.ToString()
                    ),
                    nameof(ActivityUserActor)
                )
                .BindUserWithAvailableActivitiesAsync(notification);
    }
}
