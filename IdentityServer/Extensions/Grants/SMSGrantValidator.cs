using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Entities;
using IdentityServer4.Validation;

namespace IdentityServer.Extensions.Grants
{
    public class SMSGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => CustomGrantType.SMSVerification;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            System.Console.WriteLine("Grant is working");
            try
            {
                // 参数获取
                var phone = context.Request.Raw["phone"];
                var code = context.Request.Raw["code"];

                // 通过openId和unionId 参数来进行数据库的相关验证
                var claimList = await ValidateUserAsync(phone);

                //授权通过返回
                context.Result = new GrantValidationResult
                (
                    subject: phone,
                    authenticationMethod: "SMS",
                    claims: claimList.ToArray()
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
        private async Task<List<Claim>> ValidateUserAsync(string phone)
        {
            // 数据库查询
            var user = new ApplicationUser();

            await Task.Run(() =>
            {
                // TODO
            });

            if (user == null)
            {
                //注册用户
            }

            return new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, phone),
            };
        }
    }
}
