using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs
{
    public class SmsDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}