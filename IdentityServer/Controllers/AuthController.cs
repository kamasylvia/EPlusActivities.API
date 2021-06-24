using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using IdentityServer.Helpers;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OtpNet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISmsService _smsService;
        private readonly DataProtectorTokenProvider<ApplicationUser> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public AuthController(
            IConfiguration configuration,
            ISmsService smsService,
            DataProtectorTokenProvider<ApplicationUser> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _smsService = smsService
                ?? throw new ArgumentNullException(nameof(smsService));
            _dataProtectorTokenProvider = dataProtectorTokenProvider
                ?? throw new ArgumentNullException(nameof(dataProtectorTokenProvider));
            _phoneNumberTokenProvider = phoneNumberTokenProvider
                ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }


        [AllowAnonymous]
        [HttpPost("sms")]
        public async Task<IActionResult> GetVerificationCodeAsync([FromBody] SmsDto smsDto)
        {
            var user = await _smsService.GetSmsUserAsync(smsDto);
            var phoneNumber = smsDto.PhoneNumber;

            // 用 TotpHelper 生成的验证码可以设定有效期和更新时间
            // 优点：自定义有效期和重新生成验证码的周期。
            // 缺点：用一个 Dictionary<phone, Totp> 储存数据，容易造成内存泄漏。
            // var token = TotpHelper.GetCode(phone);

            // 用 PhoneNumberTokenProvider 生成的验证码是官方实现。
            // 优点：官方范例，稳定可靠。
            // 缺点：有效期和更新时间固定
            // 有效期：9 分钟
            // 重新生成周期：3 分钟
            var token = await _phoneNumberTokenProvider.GenerateAsync(
                OidcConstants.AuthenticationMethods.ConfirmationBySms,
                _userManager,
                user);
                
            var response = await _smsService.SendAsync(phoneNumber, token);
            return Ok(response);
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            System.Console.WriteLine($"Get {id}");
            return "value";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
