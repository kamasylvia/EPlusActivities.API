using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.ActivityService
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        private readonly ILogger<ActivityService> _logger;
        private readonly IMapper _mapper;
        public ActivityService(
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            ILogger<ActivityService> logger,
            IMapper mapper
        ) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<IEnumerable<Activity>> GetActivitiesAsync(
            IEnumerable<ChannelCode> availableChannels,
            DateTime startTime,
            DateTime? endTime = null
        ) =>
            (await _activityRepository.FindActivitiesAsync(startTime, endTime)).Where(
                activity => activity.AvailableChannels.Intersect(availableChannels).Count() > 0
            );

        public async Task<IEnumerable<Activity>> GetAvailableActivitiesAsync(
            IEnumerable<ChannelCode> availableChannels,
            DateTime startTime,
            DateTime? endTime = null
        ) {
            var result = new List<Activity>();
            var activitiesAtStartTime = await _activityRepository.FindAvailableActivitiesAsync(
                startTime
            );
            var activitiesAtEndTime = await _activityRepository.FindAvailableActivitiesAsync(
                endTime ?? DateTime.Now.Date
            );

            return activitiesAtStartTime.Union(activitiesAtEndTime)
                .Where(
                    activity => activity.AvailableChannels.Intersect(availableChannels).Count() > 0
                );
        }

        public async Task<IEnumerable<ActivityUser>> BindUserWithActivities(
            Guid userId,
            IEnumerable<Guid> activityIds
        ) {
            var result = new List<ActivityUser>();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogError("用户不存在");
                return result;
            }

            var existedLinks = await _activityUserRepository.FindByParentIdAsync(userId);
            var unBoundActivityIds = activityIds.Except(
                existedLinks.Select(activityUser => activityUser.ActivityId.Value)
            );

            foreach (var activityId in unBoundActivityIds)
            {
                var activity = await _activityRepository.FindByIdAsync(activityId);
                if (activity is not null)
                {
                    var activityUser = new ActivityUser { User = user, Activity = activity };
                    result.Add(activityUser);
                    await _activityUserRepository.AddAsync(activityUser);
                }
            }
            var dbResult = await _activityUserRepository.SaveAsync();

            if (!dbResult)
            {
                _logger.LogError("绑定用户和活动失败。");
            }

            return result;
        }

        public async Task<IEnumerable<ActivityUser>> BindUserWithAvailableActivities(
            Guid userId,
            ChannelCode availableChannel
        ) =>
            await BindUserWithActivities(
                userId,
                (
                    await GetAvailableActivitiesAsync(
                        new List<ChannelCode> { availableChannel },
                        DateTime.Now.Date
                    )
                ).Select(activity => activity.Id.Value)
            );
    }
}
