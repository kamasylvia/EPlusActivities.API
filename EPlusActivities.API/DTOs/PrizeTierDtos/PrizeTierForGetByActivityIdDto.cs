using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTierDtos
{
    public class PrizeTierForGetByActivityIdDto
    {
        [Required]
        public Guid? ActivityId { get; set; }
    }
}
