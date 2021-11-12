using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Services.ActivityService
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetActivitiesAsync(
            IEnumerable<ChannelCode> availableChannels,
            DateTime startTime,
            DateTime? endTime = null
        );

        Task<IEnumerable<Activity>> GetAvailableActivitiesAsync(
            IEnumerable<ChannelCode> availableChannels,
            DateTime startTime,
            DateTime? endTime = null
        );

        Task<IEnumerable<ActivityUser>> GetAvailableActivityUserLinksAsync(
            Guid userId,
            ChannelCode channel,
            DateTime? startTime = null,
            DateTime? endTime = null
        );

        Task<IEnumerable<ActivityUser>> BindUserWithActivities(
            Guid userId,
            IEnumerable<Guid> activityIds,
            ChannelCode channel
        );

        Task<IEnumerable<ActivityUser>> BindUserWithAvailableActivities(
            Guid userId,
            ChannelCode availableChannel
        );

        void UpdateDailyLimitsAsync(ApplicationUser user, ActivityUser activityUser);
    }
}
