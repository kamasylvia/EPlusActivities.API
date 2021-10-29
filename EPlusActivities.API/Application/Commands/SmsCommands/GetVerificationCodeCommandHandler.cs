using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.IdentityServer;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.SmsCommands
{
    public class GetVerificationCodeCommandHandler
        : BaseCommandHandler,
          IRequestHandler<GetVerificationCodeCommand>
    {
        public GetVerificationCodeCommandHandler(
            ISmsService smsService,
            UserManager<ApplicationUser> userManager,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider
        ) : base(smsService, userManager, phoneNumberTokenProvider) { }

        public async Task<Unit> Handle(
            GetVerificationCodeCommand request,
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
