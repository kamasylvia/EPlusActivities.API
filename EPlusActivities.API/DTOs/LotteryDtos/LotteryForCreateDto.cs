using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForCreateDto
    {
        [Required]
        public string ChannelCode { get; set; }

        [Required]
        public string LotteryDisplay { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        [Range(1, int.MaxValue)]
        public int Count { get; set; }
    }
}
