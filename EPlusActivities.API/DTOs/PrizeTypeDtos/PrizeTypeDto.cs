using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Percentage { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
