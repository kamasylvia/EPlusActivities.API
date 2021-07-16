using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public Guid? UserId { get; set; }
    }
}
