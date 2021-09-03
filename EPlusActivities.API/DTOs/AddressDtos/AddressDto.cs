using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressDto
    {
        [Required]
        public Guid? Id { get; set; }

        // 收件人
        public string Recipient { get; set; }

        // 收件人电话
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string RecipientPhoneNumber { get; set; }

        public string Country { get; set; }

        // 省、直辖市、自治区
        public string Province { get; set; }

        public string City { get; set; }

        // 具体地址
        public string DetailedAddress { get; set; }

        // 邮编
        public string Postcode { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public bool IsDefault { get; set; }
    }
}
