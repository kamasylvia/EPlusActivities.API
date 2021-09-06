using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int? Limit { get; set; }

        public int? DailyLimit { get; set; }

        public IEnumerable<string> AvailableChannels { get; set; }

        public string LotteryDisplay { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
