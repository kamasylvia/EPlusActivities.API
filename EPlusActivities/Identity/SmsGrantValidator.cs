using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EPlusActivities.API.Identity
{
    public class SmsGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public string GrantType => OidcConstants.AuthenticationMethods.ConfirmationBySms;
        public SmsGrantValidator(
            IConfiguration configuration,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
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
                // 参数获取
                // 未来版本的 AutoMapper 可以自动获取参数。
                var phoneNumber = context.Request.Raw["phone_number"];
                var token = context.Request.Raw["token"];
                var loginChannel = context.Request.Raw["login_channel"];

                var requireRegister = false;
                var secret = new Secret(_configuration["Secrets:DefaultSecret"]).Value;

                // 这是 EF Core 内置的查询方法，速度很慢，所以用下面的 LINQ 替代。
                // var user = await _userManager.FindByNameAsync(phoneNumber);

                var user = await _userManager.Users.SingleOrDefaultAsync(x =>
                    x.PhoneNumber == phoneNumber);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = phoneNumber,
                        NormalizedUserName = phoneNumber,
                        PhoneNumber = phoneNumber,
                        SecurityStamp = secret + phoneNumber.Sha256()
                    };
                    requireRegister = true;
                }

                // 验证
                // var validationResult = TotpHelper.Validate(phoneNumber, code);
                var validationResult =
                    await _phoneNumberTokenProvider.ValidateAsync(
                        OidcConstants.AuthenticationMethods.ConfirmationBySms,
                        token,
                        _userManager,
                        user);

                System.Console.WriteLine($"Valication result = {validationResult}");

                System.Console.WriteLine($"User in validation: {user}");

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
                    System.Console.WriteLine("开始注测用户");
                    user.PhoneNumberConfirmed = true;
                    user.RegisterChannel = loginChannel;
                    user.LoginChannel = loginChannel;
                    user.RegisterDate = DateTime.Now;
                    var creationResult = await _userManager.CreateAsync(user);
                    if (!creationResult.Succeeded)
                    {
                        System.Console.WriteLine("注册失败");
                        // var errors = creationResult.Errors.Select(err =>
                        //     $"Error { err.Code} : {err.Description}");
                        context.Result = new GrantValidationResult(
                            TokenRequestErrors.InvalidGrant,
                            "Failed to sign up}");
                        return;
                    }

                    // await _userManager.AddToRoleAsync(user, "Customer".ToUpper());
                    await _userManager.AddToRolesAsync(user, new string[] { "Customer".ToUpper() });
                }

                // 直接登录
                await _signInManager.SignInAsync(user, isPersistent: false);

                System.Console.WriteLine("验证完成，返回结果。");

                context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: OidcConstants.AuthenticationMethods.ConfirmationBySms
                    );
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
            }
        }
    }
}
