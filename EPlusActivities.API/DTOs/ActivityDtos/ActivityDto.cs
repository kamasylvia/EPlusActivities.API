using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.ActivityDtos
{
    public class ActivityDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Limit { get; set; }

        public ChannelCode ChannelCode { get; set; }

        public LotteryDisplay LotteryDisplay { get; set; }

        public ActivityType ActivityType { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string BackgroundPhotoUrl { get; set; }
    }
}
