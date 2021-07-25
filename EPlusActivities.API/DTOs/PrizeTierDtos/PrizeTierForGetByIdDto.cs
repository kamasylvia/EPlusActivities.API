using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeTierDtos
{
    public class PrizeTierForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
