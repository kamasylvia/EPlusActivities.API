using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using IdentityServer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OtpNet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpClientFactory httpClientFactory
        )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [HttpPost("verification-code")]
        public async Task<IActionResult> GetVerificationCodeAsync([FromBody] VerificationCodeDto verificationCodeDto)
        {
            var phone = verificationCodeDto.PhoneNumber;
            var code = TotpHelper.GetCode(phone);

            var response = await SendVerificationCodeAsync(phone, code);

            var user = new ApplicationUser
            {
                UserName = phone,
                PhoneNumber = phone
            };

            return Ok(response);
        }

        private async Task<IActionResult> SendVerificationCodeAsync(string phoneNumber, string code)
        {
            var uriString = "http://10.10.34.202:20015/api/sms/message/send";
            var contentObject = new
            {
                messageContent = $"【农工商】验证码: {code}。您正在验证农工商用户，感谢您的支持！",
                messageKey = "sms_test",
                messageTarget = phoneNumber
            };
            var jsonContent = JsonContent.Create(contentObject);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(uriString, jsonContent);

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
