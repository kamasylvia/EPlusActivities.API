using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}