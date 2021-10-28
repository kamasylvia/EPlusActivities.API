using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Services.IdentityServer;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.SmsCommands
{
    public class BaseCommandHandler
    {
        protected readonly ISmsService _smsService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;

        public BaseCommandHandler(
            ISmsService smsService,
            UserManager<ApplicationUser> userManager,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider
        )
        {
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _phoneNumberTokenProvider =
                phoneNumberTokenProvider
                ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
        }
    }
}
