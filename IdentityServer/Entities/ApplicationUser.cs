using System.Text.Json;
using Microsoft.AspNetCore.Identity;


namespace IdentityServer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string RegisterChannel { get; set; }
        public string LoginChannel { get; set; }

        public override string ToString() =>
            JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { WriteIndented = true });
    }
}