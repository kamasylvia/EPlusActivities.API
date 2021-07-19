using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForCreateDto
    {
        [Required]
        public string Name { get; set; }

        public int Percentage { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}