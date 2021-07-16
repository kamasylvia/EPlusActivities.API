using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeItemDtos
{
    public class PrizeItemForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
