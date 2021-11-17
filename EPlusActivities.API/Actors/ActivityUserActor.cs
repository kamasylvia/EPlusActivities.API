using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.ActivityService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Actors
{
    public class ActivityUserActor : Actor, IActivityUserActor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ActivityService _activityService;
        private readonly IMapper _mapper;

        public ActivityUserActor(
            ActorHost host,
            UserManager<ApplicationUser> userManager,
            ActivityService activityService,
            IMapper mapper
        ) : base(host)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityService =
                activityService ?? throw new ArgumentNullException(nameof(activityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ActivityUserDto>> GetActivitiesByUserIdAsync(
            GetActivityUserByUserIdCommand request
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var bound = (await StateManager.TryGetStateAsync<bool>("bound")).Value;
            #endregion

            return bound
              ? _mapper.Map<IEnumerable<ActivityUserDto>>(
                    await _activityService.GetAvailableActivityUserLinksAsync(
                        request.UserId.Value,
                        request.AvailableChannel,
                        request.StartTime,
                        request.EndTime
                    )
                )
              : new List<ActivityUserDto>();
        }

        public async Task<bool> BindUserWithAvailableActivitiesAsync(LoginCommand request)
        {
            var bound = (await StateManager.TryGetStateAsync<bool>("bound")).Value;
            if (!bound)
            {
                bound = await StateManager.AddOrUpdateStateAsync(
                    "bound",
                    true,
                    (key, currentState) => true
                );

                var newCreatedLinks = await _activityService.BindUserWithAvailableActivities(
                    request.UserId.Value,
                    request.ChannelCode
                );
            }
            return bound;
        }
    }
}
