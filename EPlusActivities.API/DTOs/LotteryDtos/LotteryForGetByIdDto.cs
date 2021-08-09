using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
