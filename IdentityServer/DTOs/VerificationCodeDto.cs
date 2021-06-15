using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs
{
    public class VerificationCodeDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}