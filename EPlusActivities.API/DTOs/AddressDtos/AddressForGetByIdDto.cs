using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.AddressDtos
{
    public class AddressForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
