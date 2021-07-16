using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.AddressDtos
{
    public class AddressForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
