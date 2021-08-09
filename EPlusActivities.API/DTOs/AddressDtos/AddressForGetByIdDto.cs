using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressForGetByIdDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
