using System;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application
{
    public abstract class UserRequestHandlerBase
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected UserRequestHandlerBase(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
    }
}
