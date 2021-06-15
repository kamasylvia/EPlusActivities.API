using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Entities;
using IdentityServer.Helpers;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Extensions.Grants
{
    public class SMSGrantValidator : IExtensionGrantValidator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SMSGrantValidator(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string GrantType => CustomGrantType.SMSVerification;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            System.Console.WriteLine("Grant is working");
            try
            {
                // 参数获取
                var phoneNumber = context.Request.Raw["phone_number"];
                var code = context.Request.Raw["code"];

                // 通过openId和unionId 参数来进行数据库的相关验证
                var claimList = await ValidateUserAsync(phoneNumber, code);

                //授权通过返回
                context.Result = new GrantValidationResult
                (
                    subject: phoneNumber,
                    authenticationMethod: "SMS",
                    claims: new List<Claim>()
                );

            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult()
                {
                    IsError = true,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<List<Claim>> ValidateUserAsync(string phoneNumber, string code)
        {
            var validate = TotpHelper.Validate(phoneNumber, code);

            if (validate)
            {
                //注册用户
                System.Console.WriteLine("验证通过！");
            }
            else
            {
                System.Console.WriteLine("验证失败！");
            }

            return new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, phoneNumber),
            };
        }
    }
}
