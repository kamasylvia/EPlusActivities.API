using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
