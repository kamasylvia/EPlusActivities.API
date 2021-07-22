using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForGetByActivityIdDto
    {
        [Required]
        public Guid? ActivityId { get; set; }
    }
}
