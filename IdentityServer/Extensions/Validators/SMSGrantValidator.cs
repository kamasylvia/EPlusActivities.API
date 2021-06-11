using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Services.Authentication;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.Extensions.Validators
{
    public class SMSGrantValidator : IExtensionGrantValidator
    {
        public readonly IVerificationCodeService _verificationCodeService;
        public readonly IUserService _userService;

        public string GrantType => "sms";
        public SMSGrantValidator(IVerificationCodeService verificationCodeService, IUserService userService)
        {
            _verificationCodeService = verificationCodeService;
            _userService = userService;
        }


        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["sms_verification_code"];
            var error = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(code))
            {
                //用户检查
                _verificationCodeService.Validate(phone, code);
                //用户注册
                var userId = await _userService.CheckOrCreate(phone);
                if (userId <= 0)
                {
                    context.Result = error;
                    return;
                }

                context.Result = new GrantValidationResult(
                        subject: userId.ToString(),
                        authenticationMethod: GrantType
                    );
            }
            else
            {
                context.Result = error;

            }

        }

    }
}