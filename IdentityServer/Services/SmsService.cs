using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class SmsService : ISmsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public SmsService(
            IHttpClientFactory httpClientFactory,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApplicationUser> GetSmsUserAsync(SmsDto smsDto) =>
            await _userManager.FindByNameAsync(smsDto.PhoneNumber)
                ?? new ApplicationUser
                {
                    UserName = smsDto.PhoneNumber,
                    PhoneNumber = smsDto.PhoneNumber,
                    SecurityStamp = new Secret("secret").Value + smsDto.PhoneNumber.Sha256()
                };

        public async Task<HttpResponseMessage> SendAsync(string phoneNumber, string token)
        {
            var uriString = "http://10.10.34.202:20015/api/sms/message/send";
            var contentObject = new
            {
                messageContent = $"【农工商】验证码: {token}。您正在验证农工商用户，10 分钟内同一手机号最多发送 3 次验证码，每个验证码有效期为 8 分钟，感谢您的支持！",
                messageKey = "sms_test",
                messageTarget = phoneNumber
            };
            var jsonContent = JsonContent.Create(contentObject);
            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.PostAsync(uriString, jsonContent);
        }
    }
}