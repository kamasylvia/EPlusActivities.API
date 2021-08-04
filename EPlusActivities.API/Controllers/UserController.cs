using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.MemberDtos;
using EPlusActivities.API.DTOs.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IHttpClientFactory httpClientFactory,
            ILogger<UserController> logger,
            IMapper mapper,
            IMemberService memberService
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        // [Authorize(Roles = "customer")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<UserDto>> GetAsync([FromBody] UserForLoginDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            #region Get member info
            var (getMemberSucceed, memberDto) = await _memberService.GetMemberAsync(
                user.PhoneNumber
            );
            if (getMemberSucceed)
            {
                user.IsMember = true;
                user.MemberId = memberDto.Body.Content.MemberId;
                user.Credit = memberDto.Body.Content.Points;
            }
            #endregion

            #region Update the user
            user.LoginChannel = userDto.LoginChannel;
            var result = await _userManager.UpdateAsync(user);
            #endregion

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<UserDto>(user));
            }

            _logger.LogError("Failed to update the user.");
            return new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPatch("channel")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateLoginChannelAsync([FromBody] UserForLoginDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            user.LoginChannel = userDto.LoginChannel;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            _logger.LogError("Failed to update the login channel.");
            return new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPatch("phonenumber")]
        // [Authorize(Roles = "test")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdatePhoneNumberAsync(
            [FromBody] UserForUpdatePhoneDto userDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            if (user.PhoneNumber == userDto.PhoneNumber)
            {
                return Conflict("The new phone number is the same as the old one.");
            }
            #endregion

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                userDto.PhoneNumber
            );
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                userDto.PhoneNumber,
                token
            );
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to change phone number.");
                return new InternalServerErrorObjectResult(result.Errors);
            }

            result = await _userManager.SetUserNameAsync(user, userDto.PhoneNumber);
            if (result.Succeeded)
            {
                return Ok();
            }

            _logger.LogError("Failed to set UserName.");
            return new InternalServerErrorObjectResult(result.Errors);
        }

        /*      
        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> CreateAsync([FromBody] UserDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is not null)
            {
                return BadRequest("The user is already existed.");
            }
            #endregion

            // Not Completed
            user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        // [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> UpdateAsync([FromBody] UserDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            user = _mapper.Map<UserDto, ApplicationUser>(userDto, user);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }
        */

        [HttpDelete]
        // [Authorize(Roles = "admin")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] UserForDeleteDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            _logger.LogError("Failed to delete the user.");
            return new InternalServerErrorObjectResult(result.Errors);
        }
    }
}
