using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForCreateDto
    {
        [Required]
        public Channel Channel { get; set; }

        public bool IsLucky { get; set; }

        public int UsedCredit { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Guid? PrizeItemId { get; set; }
    }
}
