using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual IEnumerable<ApplicationUserRole> UserRoles { get; set; }
    }
}
