using System;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EPlusActivities.API.Services.IdentityServer
{
    public class SmsGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IConfiguration _configuration;

        private readonly PhoneNumberTokenProvider<ApplicationUser>
            _phoneNumberTokenProvider;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public string GrantType =>
            OidcConstants.AuthenticationMethods.ConfirmationBySms;

        public SmsGrantValidator(
            IConfiguration configuration,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _configuration = configuration;
            _phoneNumberTokenProvider = phoneNumberTokenProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                #region 参数获取
                // 未来版本的 AutoMapper 可以自动获取参数。
                var phoneNumber = context.Request.Raw["phone_number"];
                var token = context.Request.Raw["token"];
                var loginChannel = context.Request.Raw["login_channel"];

                var requireRegister = false;
                var secret =
                    new Secret(_configuration["Secrets:DefaultSecret"]).Value;
                #endregion



                #region 获取用户
                // 这是 EF Core 内置的查询方法，速度很慢，所以用下面的 LINQ 替代。
                // var user = await _userManager.FindByNameAsync(phoneNumber);
                var user =
                    await _userManager
                        .Users
                        .SingleOrDefaultAsync(x =>
                            x.PhoneNumber == phoneNumber);

                if (user == null)
                {
                    user =
                        new ApplicationUser
                        {
                            UserName = phoneNumber,
                            NormalizedUserName = phoneNumber,
                            PhoneNumber = phoneNumber,
                            SecurityStamp = secret + phoneNumber.Sha256()
                        };
                    requireRegister = true;
                }
                #endregion



                #region 验证
                var validationResult =
                    await _phoneNumberTokenProvider
                        .ValidateAsync(OidcConstants
                            .AuthenticationMethods
                            .ConfirmationBySms,
                        token,
                        _userManager,
                        user);

                if (!validationResult)
                {
                    context.Result =
                        new GrantValidationResult(TokenRequestErrors
                                .InvalidGrant,
                            "Failed to validate SMS token.");
                    return;
                }
                #endregion



                #region 注册
                if (requireRegister)
                {
                    user.PhoneNumberConfirmed = true;
                    user.RegisterChannel = Enum.Parse<ChannelCode>(loginChannel);
                    user.LoginChannel = Enum.Parse<ChannelCode>(loginChannel);
                    user.RegisterDate = DateTime.Now;
                    var creationResult = await _userManager.CreateAsync(user);
                    if (!creationResult.Succeeded)
                    {
                        context.Result =
                            new GrantValidationResult(TokenRequestErrors
                                    .InvalidGrant,
                                "Failed to register.");
                        return;
                    }

                    // await _userManager.AddToRoleAsync(user, "Customer".ToUpper());
                    await _userManager
                        .AddToRolesAsync(user,
                        new string[] { "Customer".ToUpper() });
                }
                #endregion



                #region 登录
                /*
                    非常重要！
                    验证方法中 IdentityUser 直接登录才能使得 API 的授权认证成功执行。
                    否则访问受保护的 API 将返回 404。
                */
                await _signInManager.SignInAsync(user, isPersistent: false);
                #endregion


                context.Result =
                    new GrantValidationResult(subject: user.Id.ToString(),
                        authenticationMethod: OidcConstants
                            .AuthenticationMethods
                            .ConfirmationBySms);
            }
            catch (Exception ex)
            {
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                        ex.Message);
            }
        }
    }
}
