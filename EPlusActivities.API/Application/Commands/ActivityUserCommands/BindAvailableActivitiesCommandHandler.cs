using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class BindAvailableActivitiesCommandHandler
        : IRequestHandler<BindAvailableActivitiesCommand, IEnumerable<ActivityUserDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public BindAvailableActivitiesCommandHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            BindAvailableActivitiesCommand command,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(command.UserId.ToString() + command.ChannelCode.ToString()),
                    nameof(ActivityUserActor)
                )
                .BindUserWithAvailableActivitiesAsync(command);
    }
}
