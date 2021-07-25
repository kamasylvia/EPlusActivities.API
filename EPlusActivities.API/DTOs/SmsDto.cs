using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class SmsDto
    {
        [Required]
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string PhoneNumber { get; set; }
    }
}
