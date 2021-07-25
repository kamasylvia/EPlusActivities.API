using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForCreateDto
    {
        public bool IsLucky { get; set; }

        [Required]
        public ChannelCode? ChannelCode { get; set; }

        [Required]
        public LotteryCode? LotteryCode { get; set; }

        public int UsedCredit { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
