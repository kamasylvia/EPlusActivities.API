using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.DTOs
{
    public class SmsDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}