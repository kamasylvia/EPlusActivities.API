using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTypeDtos
{
    public class PrizeTypeForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int Percentage { get; set; }
    }
}
