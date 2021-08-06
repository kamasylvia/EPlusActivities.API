using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForCreateDto
    {
        [Required]
        public ChannelCode? ChannelCode { get; set; }

        [Required]
        public LotteryDisplay? LotteryDisplay { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        [Range(1, int.MaxValue)]
        public int Count { get; set; }
    }
}
