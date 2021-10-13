using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeTierDtos
{
    public class PrizeTierForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
