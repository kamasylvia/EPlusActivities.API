using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class SmsDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}