using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeTierDtos
{
    public class PrizeTierForGetByActivityIdDto
    {
        [Required]
        public Guid? ActivityId { get; set; }
    }
}
