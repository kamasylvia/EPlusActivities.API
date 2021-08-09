using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForUpdatePhoneDto
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        [Phone]
        [StringLength(
            11,
            MinimumLength = 11,
            ErrorMessage = "A valid phone number must be 11 digits."
        )]
        public string PhoneNumber { get; set; }
    }
}
