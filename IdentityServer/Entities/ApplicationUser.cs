using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string RegisterChannel { get; set; }
        public string LoginChannel { get; set; }
    }
}