using System;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Entities
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}