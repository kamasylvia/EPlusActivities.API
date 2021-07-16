using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeItemDtos
{
    public class PrizeItemForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
