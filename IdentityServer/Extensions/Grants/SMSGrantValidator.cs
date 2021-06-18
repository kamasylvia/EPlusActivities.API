using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Entities;
using IdentityServer.Helpers;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Extensions.Grants
{
    public class SmsGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public string GrantType => OidcConstants.AuthenticationMethods.ConfirmationBySms;
        public SmsGrantValidator(
            IConfiguration configuration,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _phoneNumberTokenProvider = phoneNumberTokenProvider;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                // 参数获取
                var phoneNumber = context.Request.Raw["phone_number"];
                var token = context.Request.Raw["token"];
                // var channel = context.Request.Raw["channel"];

                var requireRegister = false;
                var user = await _userManager.FindByNameAsync(phoneNumber);
                var secret = new Secret(_configuration["Secrets:DefaultSecret"]).Value;

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = phoneNumber,
                        PhoneNumber = phoneNumber,
                        SecurityStamp = secret + phoneNumber.Sha256()
                    };
                    requireRegister = true;
                }

                // 验证
                // var validationResult = TotpHelper.Validate(phoneNumber, code);
                var validationResult =
                    await _phoneNumberTokenProvider.ValidateAsync(
                        "sms",
                        token,
                        _userManager,
                        user);


                if (!validationResult)
                {
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidGrant,
                        "Invalidate failed.");
                    return;
                }

                // 注册
                if (requireRegister)
                {
                    user.PhoneNumberConfirmed = true;
                    var creationResult = await _userManager.CreateAsync(user);
                    if (!creationResult.Succeeded)
                    {
                        context.Result = new GrantValidationResult(
                            TokenRequestErrors.InvalidGrant,
                            "Failed to sign up.");
                        return;
                    }
                }

                // 登录
                await _signInManager.SignInAsync(user, true);

                //授权通过返回
                var claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Role,"customer")
                    };

                context.Result = new GrantValidationResult(
                    subject: user.Id,
                    authenticationMethod: OidcConstants.AuthenticationMethods.ConfirmationBySms,
                    claims: claims);
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
        }
    }
}
