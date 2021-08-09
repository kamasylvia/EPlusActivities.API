using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EPlusActivities.API.Services.IdentityServer
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public SmsService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            UserManager<ApplicationUser> userManager
        ) {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApplicationUser> GetSmsUserAsync(SmsDto smsDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(
                x => x.PhoneNumber == smsDto.PhoneNumber
            );

            if (user == null)
            {
                user = _mapper.Map<ApplicationUser>(smsDto);
                user.SecurityStamp =
                    new Secret(_configuration["Secrets:DefaultSecret"]).Value
                    + smsDto.PhoneNumber.Sha256();
            }

            return user;
        }

        public async Task<HttpResponseMessage> SendAsync(string phoneNumber, string token)
        {
            var requestUri = "http://10.10.34.202:20015/api/sms/message/send";
            var contentObject = new
            {
                messageContent = $"【农工商】验证码: {token}。您正在验证农工商用户，10 分钟内同一手机号最多发送 3 次验证码，每个验证码有效期为 8 分钟，感谢您的支持！",
                messageKey = "sms_test",
                messageTarget = phoneNumber
            };
            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.PostAsJsonAsync(requestUri, contentObject);
        }
    }
}
