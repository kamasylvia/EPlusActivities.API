using System;
using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.IdentityServer;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.SmsQueries
{
    public class GetVerificationCodeQueryHandler : IRequestHandler<GetVerificationCodeQuery>
    {
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;

        public GetVerificationCodeQueryHandler(
            ISmsService smsService,
            UserManager<ApplicationUser> userManager,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider
        )
        {
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _userManager = userManager;
            _phoneNumberTokenProvider = phoneNumberTokenProvider;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _phoneNumberTokenProvider =
                phoneNumberTokenProvider
                ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _smsService = smsService;
        }

        public async Task<Unit> Handle(
            GetVerificationCodeQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await _smsService.GetSmsUserAsync(request);
            var phoneNumber = request.PhoneNumber;

            // 有效期：9 分钟
            // 重新生成周期：3 分钟
            var token = await _phoneNumberTokenProvider.GenerateAsync(
                OidcConstants.AuthenticationMethods.ConfirmationBySms,
                _userManager,
                user
            );

            var response = await _smsService.SendAsync(phoneNumber, token);
            if (!response.IsSuccessStatusCode)
            {
                throw new RemoteServiceException("Sms service error.");
            }

            return Unit.Value;
        }
    }
}
