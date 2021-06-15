using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs
{
    public class VerifyDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}