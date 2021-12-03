﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.ActivityService
{
    [CustomDependency(ServiceLifetime.Scoped)]
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityUserRepository _activityUserRepository;
        private readonly ILogger<ActivityService> _logger;
        private readonly IMapper _mapper;
        public ActivityService(
            IActivityRepository activityRepository,
            UserManager<ApplicationUser> userManager,
            IActivityUserRepository activityUserRepository,
            ILogger<ActivityService> logger,
            IMapper mapper
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public void UpdateDailyLimitsAsync(ApplicationUser user, ActivityUser activityUser)
        {
            if (!(user.LastLoginDate >= DateTime.Today))
            {
                user.LastLoginDate = DateTime.Today;
                activityUser.TodayUsedDraws = 0;
                activityUser.TodayUsedRedempion = 0;
            }
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
        )
        {
            var result = new List<Activity>();
            var activitiesAtStartTime = await _activityRepository.FindAvailableActivitiesAsync(
                startTime
            );
            var activitiesAtEndTime = await _activityRepository.FindAvailableActivitiesAsync(
                endTime ?? DateTime.Now.Date
            );
            /*
            System.Console.WriteLine(
                $"availableChannels = {JsonSerializer.Serialize(availableChannels)}"
            );
            System.Console.WriteLine(
                $"activitiesAtStartTime = {JsonSerializer.Serialize(activitiesAtStartTime)}"
            );
            System.Console.WriteLine(
                $"activitiesAtEndTime = {JsonSerializer.Serialize(activitiesAtEndTime)}"
            );
            System.Console.WriteLine(
                $"availableActivities = {JsonSerializer.Serialize(activitiesAtStartTime.Union(activitiesAtEndTime).Where(activity => activity.AvailableChannels.Intersect(availableChannels).Count() > 0))}"
            );
            */

            return activitiesAtStartTime
                .Union(activitiesAtEndTime)
                .Where(
                    activity => activity.AvailableChannels.Intersect(availableChannels).Count() > 0
                );
        }

        public async Task<IEnumerable<ActivityUser>> GetAvailableActivityUserLinksAsync(
            Guid userId,
            ChannelCode channel,
            DateTime? startTime = null,
            DateTime? endTime = null
        )
        {
            var result = new List<ActivityUser>();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogError("用户不存在");
                return result;
            }

#if DEBUG
            // System.Console.WriteLine(
            //     $"_activityUserRepository.FindByUserIdAsync(userId) = {JsonSerializer.Serialize(await _activityUserRepository.FindByUserIdAsync(userId))}"
            // );
            var temp = (await _activityUserRepository.FindByUserIdAsync(userId)).Where(
                au =>
                    !(au.Activity.StartTime > (startTime ?? DateTime.Today))
                    && !((endTime ?? DateTime.Today) > au.Activity.EndTime)
                    // && au.Channel == channel
            );
            System.Console.WriteLine($"channel = {channel}");
            System.Console.WriteLine(temp.Count());
#endif

            return (await _activityUserRepository.FindByUserIdAsync(userId)).Where(
                au =>
                    !(au.Activity.StartTime > (startTime ?? DateTime.Today))
                    && !((endTime ?? DateTime.Today) > au.Activity.EndTime)
                    && au.Channel == channel
            );
        }

        public async Task<IEnumerable<ActivityUser>> BindUserWithActivities(
            Guid userId,
            IEnumerable<Guid> activityIds,
            ChannelCode channel
        )
        {
#if DEBUG
            System.Console.WriteLine("Enter BindUserWithActivities");
#endif

            var result = new List<ActivityUser>();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogError("用户不存在");
                return result;
            }

#if DEBUG
            System.Console.WriteLine("Before existedLinks");
#endif

            var existedLinks = (await _activityUserRepository.FindByUserIdAsync(userId)).Where(
                au => au.Channel == channel
            );

#if DEBUG
            // System.Console.WriteLine($"esistedLinks = {JsonSerializer.Serialize(existedLinks)}");
#endif

#if DEBUG
            System.Console.WriteLine("Before unBoundActivityIds");
#endif
            var unBoundActivityIds = activityIds.Except(
                existedLinks.Select(activityUser => activityUser.ActivityId.Value)
            );

#if DEBUG
            System.Console.WriteLine(
                $"unBoundActivityIds = {JsonSerializer.Serialize(unBoundActivityIds)}"
            );
#endif

            foreach (var activityId in unBoundActivityIds)
            {
                var activity = await _activityRepository.FindByIdAsync(activityId);
                if (activity is not null)
                {
                    var activityUser = new ActivityUser
                    {
                        User = user,
                        Activity = activity,
                        Channel = channel
                    };
                    result.Add(activityUser);
                }
            }

            await _activityUserRepository.AddRangeAsync(result);
            var dbResult = await _activityUserRepository.SaveAsync();

            if (!dbResult)
            {
                _logger.LogError("绑定用户和活动失败。");
            }

            result.AddRange(existedLinks);
            return result;
        }

        public async Task<IEnumerable<ActivityUser>> BindUserWithAvailableActivities(
            Guid userId,
            ChannelCode channel
        ) =>
            await BindUserWithActivities(
                userId,
                (
                    await GetAvailableActivitiesAsync(
                        new List<ChannelCode> { channel },
                        DateTime.Now.Date
                    )
                ).Select(activity => activity.Id.Value),
                channel
            );
    }
}
