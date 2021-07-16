using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        // private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public UserController(
            UserManager<ApplicationUser> userManager,
            // SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            // _signInManager = signInManager
            //     ?? throw new ArgumentNullException(nameof(signInManager));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        // [Authorize(Roles = "customer")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<UserDto>> GetAsync([FromBody] UserForLoginDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            return user is null
                ? NotFound("Could not find the user.")
                : Ok(_mapper.Map<UserDto>(user));
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
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }

        [HttpPatch("phonenumber")]
        // [Authorize(Roles = "test")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdatePhoneNumberAsync([FromBody] UserForUpdatePhoneDto userDto)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(userDto.UserId.ToString());
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
                userDto.PhoneNumber);
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                userDto.PhoneNumber,
                token);
            if (!result.Succeeded)
            {
                return new InternalServerErrorObjectResult(result.Errors);
            }

            result = await _userManager.SetUserNameAsync(user, userDto.PhoneNumber);
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
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
            return result.Succeeded ? Ok() : new InternalServerErrorObjectResult(result.Errors);
        }
    }
}
