using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
