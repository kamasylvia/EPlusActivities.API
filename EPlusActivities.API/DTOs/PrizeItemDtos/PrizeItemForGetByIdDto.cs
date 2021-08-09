using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeItemDtos
{
    public class PrizeItemForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
