using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
