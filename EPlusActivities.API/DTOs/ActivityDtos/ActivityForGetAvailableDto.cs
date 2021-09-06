using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.ActivityDtos
{
    public class ActivityForGetAvailableDto
    {
        [Required]
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public IEnumerable<string> AvailableChannels { get; set; }
    }
}
