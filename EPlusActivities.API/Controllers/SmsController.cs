using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : Controller
    {
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;

        public SmsController(
            ISmsService smsService,
            UserManager<ApplicationUser> userManager,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider)
        {
            _smsService = smsService
                ?? throw new ArgumentNullException(nameof(smsService));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _phoneNumberTokenProvider = phoneNumberTokenProvider
                ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetVerificationCodeAsync([FromBody] SmsDto smsDto)
        {
            var user = await _smsService.GetSmsUserAsync(smsDto);
            var phoneNumber = smsDto.PhoneNumber;

            // 有效期：9 分钟
            // 重新生成周期：3 分钟
            var token = await _phoneNumberTokenProvider.GenerateAsync(
                OidcConstants.AuthenticationMethods.ConfirmationBySms,
                _userManager,
                user);

            var response = await _smsService.SendAsync(phoneNumber, token);
            return Ok(response);
        }
    }
}
