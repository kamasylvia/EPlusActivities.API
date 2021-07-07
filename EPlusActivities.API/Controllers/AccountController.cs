﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAddressRepository _addressRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public UserController(
            UserManager<ApplicationUser> userManager,
            IAddressRepository addressRepository,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
            _addressRepository = addressRepository;
        }

        [HttpGet("[action]")]
        // [Authorize(Roles = "customer")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByIdAsync(loginDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return await GetUserAsync(loginDto);
        }


        [HttpGet]
        // [Authorize(Roles = "customer")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<UserDto>> GetUserAsync([FromBody] LoginDto loginDto)
        {
            var userId = loginDto.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.Addresses = (await _addressRepository.FindByUserIdAsync(userId)).ToList();


            return user is null
                ? NotFound("用户不存在")
                : Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPatch("phonenumber")]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] UserDto userDto)
        {
            if (userDto.PhoneNumber is null)
            {
                return BadRequest("手机号不能为空");
            }

            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                userDto.PhoneNumber);
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                userDto.PhoneNumber,
                token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            result = await _userManager.SetUserNameAsync(user, userDto.PhoneNumber);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto userDto)
        {
            // Not Completed
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpPut]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDto userDto)
        {
            // Not Completed
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }


        [HttpDelete]
        // [Authorize(Roles = "admin")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
        }
    }
}