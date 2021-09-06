using System;
using System.Collections.Generic;
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
        public IEnumerable<string> AvailableChannels { get; set; }

        [Required]
        public string LotteryDisplay { get; set; }

        [Required]
        public string ActivityType { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
