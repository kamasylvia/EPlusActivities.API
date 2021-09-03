using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressForCreateDto
    {
        public string Recipient { get; set; }

        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string RecipientPhoneNumber { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string DetailedAddress { get; set; }

        public string Postcode { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public bool IsDefault { get; set; }
    }
}
