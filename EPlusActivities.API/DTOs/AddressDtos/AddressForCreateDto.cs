using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.AddressDtos
{
    public class AddressForCreateDto
    {
        /// <summary>
        /// 收件人
        /// </summary>
        /// <value></value>
        public string Recipient { get; set; }

        /// <summary>
        /// 收件人手机号
        /// </summary>
        /// <value></value>
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string RecipientPhoneNumber { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        /// <value></value>
        public string Country { get; set; }

        /// <summary>
        /// 省、自治区、直辖市
        /// </summary>
        /// <value></value>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        /// <value></value>
        public string City { get; set; }

        /// <summary>
        /// 具体地址
        /// </summary>
        /// <value></value>
        public string DetailedAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        /// <value></value>
        public string Postcode { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 是否为默认地址
        /// </summary>
        /// <value></value>
        public bool IsDefault { get; set; }
    }
}
