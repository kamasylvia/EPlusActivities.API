using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public bool PickedUp { get; set; }

        public bool Delivered { get; set; }

        public DateTime? PickedUpTime { get; set; }
    }
}
