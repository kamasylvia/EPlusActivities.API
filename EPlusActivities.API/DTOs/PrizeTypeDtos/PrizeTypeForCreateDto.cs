using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForCreateDto
    {
        [Required]
        public string Name { get; set; }

        public int Percentage { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public IEnumerable<Guid> PrizeItemIds { get; set; }
    }
}
