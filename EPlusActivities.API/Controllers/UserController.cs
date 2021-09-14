using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 用户账户 API
    /// </summary>
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

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
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
            if ((await _userManager.GetRolesAsync(user)).Contains("customer"))
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
            var result = await _userManager.UpdateAsync(user);
            #endregion

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<UserDto>(user));
            }

            _logger.LogError("Failed to update the user.");
            return new InternalServerErrorObjectResult(result.Errors);
        }

        /// <summary>
        /// 获取用户列表。
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "ManagerPolicy"
        )]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync(
            [FromQuery] UserForGetUsersDto requestDto
        ) {
            var userList = (
                await _userManager.GetUsersInRoleAsync(requestDto.Role.Trim().ToLower())
            ).ToList();

            var startIndex = (requestDto.PageIndex - 1) * requestDto.PageSize;
            var count = requestDto.PageIndex * requestDto.PageSize;

            if (userList.Count < startIndex)
            {
                return NotFound("Could not find any users.");
            }

            if (userList.Count - startIndex < count)
            {
                count = userList.Count - startIndex;
            }

            var result = userList.GetRange(startIndex, count);
            return result.Count > 0
                ? _mapper.Map<List<UserDto>>(result)
                : NotFound("Could not find any users.");
        }

        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPatch("phonenumber")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "CustomerPolicy"
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

        /// <summary>
        /// 新建管理员账户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AdminPolicy"
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
            result = await _userManager.AddToRoleAsync(user, userDto.Role.ToLower());
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

        /// <summary>
        /// 删除账户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AdminPolicy"
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
