using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.ActivityUserQueries
{
    public class GetActivityUserByUserIdQueryHandler
        : IRequestHandler<GetActivityUserByUserIdQuery, IEnumerable<ActivityUserDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public GetActivityUserByUserIdQueryHandler(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            GetActivityUserByUserIdQuery request,
            CancellationToken cancellationToken
        ) =>
            await _actorProxyFactory
                .CreateActorProxy<IActivityUserActor>(
                    new ActorId(request.UserId.ToString() + request.AvailableChannel.ToString()),
                    nameof(ActivityUserActor)
                )
                .GetActivitiesByUserIdAsync(request);
    }
}
