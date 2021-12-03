using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.ActivityService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.ActivityUserQueries
{
    public class GetActivityUserByUserIdQueryHandler
        : IRequestHandler<GetActivityUserByUserIdQuery, IEnumerable<ActivityUserDto>>
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;

        public GetActivityUserByUserIdQueryHandler(
            IActorProxyFactory actorProxyFactory,
            UserManager<ApplicationUser> userManager,
            IActivityService activityService,
            IMapper mapper)
        {
            _actorProxyFactory =
                actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
            _userManager = userManager;
            _activityService = activityService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityUserDto>> Handle(
            GetActivityUserByUserIdQuery request,
            CancellationToken cancellationToken
        )
        /*
      =>
        await _actorProxyFactory
            .CreateActorProxy<IActivityUserActor>(
                new ActorId(request.UserId.ToString() + request.AvailableChannel.ToString()),
                nameof(ActivityUserActor)
            )
            .GetActivitiesByUserIdAsync(request);
            */
        {

            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            return _mapper.Map<IEnumerable<ActivityUserDto>>(
                await _activityService.GetAvailableActivityUserLinksAsync(
                    request.UserId.Value,
                    request.AvailableChannel,
                    request.StartTime,
                    request.EndTime
                )
            );

        }

    }
}
