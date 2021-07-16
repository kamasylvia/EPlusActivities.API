using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
