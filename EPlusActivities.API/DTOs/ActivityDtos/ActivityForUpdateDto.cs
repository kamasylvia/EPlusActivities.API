using System;
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

        public ChannelCode ChannelCode { get; set; }

        public LotteryDisplay LotteryDisplay { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string BackgroundPhotoUrl { get; set; }
    }
}
