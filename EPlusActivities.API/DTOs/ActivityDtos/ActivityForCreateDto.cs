using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForCreateDto
    {
        [Required]
        public string Name { get; set; }

        public int? Limit { get; set; }

        public int? DailyLimit { get; set; }

        [Required]
        public ChannelCode ChannelCode { get; set; }

        [Required]
        public LotteryDisplay LotteryDisplay { get; set; }

        [Required]
        public ActivityType ActivityType { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
