using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressForGetByUserIdDto
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
