using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Application.Queries.SmsQueries;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Services.IdentityServer
{
    [CustomDependency(ServiceLifetime.Scoped)]
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
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<ApplicationUser> GetSmsUserAsync(GetVerificationCodeQuery smsDto)
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
            var requestUri = _configuration["SendSmsApi:Url"];
            var contentObject = new
            {
                messageContent = $"【农工商】验证码: {token}。您正在验证农工商用户，10 分钟内同一手机号最多发送 3 次验证码，每个验证码有效期为 8 分钟，感谢您的支持！",
                messageKey = _configuration["SendSmsApi:MessageKey"],
                messageTarget = phoneNumber
            };
            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.PostAsJsonAsync(requestUri, contentObject);
        }
    }
}
