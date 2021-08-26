using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<UserDto>> GetAsync([FromQuery] UserForLoginDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            #region Get member info
            if (userDto.LoginChannel is ChannelCode.MiniProgram)
            {
                var (getMemberSucceed, memberDto) = await _memberService.GetMemberAsync(
                    user.PhoneNumber
                );
                if (getMemberSucceed)
                {
                    user.IsMember = true;
                    user.MemberId = memberDto.Body.Content.MemberId;
                    user.Credit = memberDto.Body.Content.Points;
                }
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
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
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
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
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

        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> CreateAdminOrManagerAsync(
            [FromBody] UserForCreateAdminDto userDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user is not null)
            {
                return Conflict("The user is already existed.");
            }
            #endregion

            user = new ApplicationUser { UserName = userDto.UserName };
            var result = await _userManager.CreateAsync(user, userDto.Password);
            result = await _userManager.AddToRoleAsync(user, userDto.Role);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        /*
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
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
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
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
