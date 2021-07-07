using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        public AddressController(
            UserManager<ApplicationUser> userManager,
            IAddressRepository addressRepository,
            IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetByUserIdAsync([FromBody] AddressDto addressDto)
        {
            var userId = addressDto.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user is null
                ? BadRequest(new ArgumentNullException("用户不存在"))
                : Ok(_mapper.Map<IEnumerable<AddressDto>>(user.Addresses));
        }

        [HttpGet("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<ActionResult<AddressDto>> GetByIdAsync([FromBody] AddressDto addressDto)
        {
            var address = await _addressRepository.FindByIdAsync(addressDto.Id);
            return address is null ? BadRequest(new ArgumentException("地址不存在，请检查传入的地址 id")) : Ok(address);
        }

        [HttpPost("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<ActionResult<AddressDto>> AddAddressAsync([FromBody] AddressDto addressDto)
        {
            // 参数验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // 参数验证
            if (await _addressRepository.ExistsAsync(addressDto.Id))
            {
                return BadRequest(new ArgumentException("地址已存在"));
            }

            // 参数验证
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest(new ArgumentNullException("用户不存在"));
            }

            // 参数验证
            var oldAddresses = await _addressRepository.FindByUserIdAsync(user.Id);
            if (oldAddresses?.Count() >= 5)
            {
                return BadRequest(new OverflowException("用户不可添加超过 5 个地址"));
            }

            // 存入数据库
            var address = _mapper.Map<Address>(addressDto);
            await _addressRepository.AddAsync(address);
            var result = await _addressRepository.SaveAsync();
            if (!result)
            {
                return BadRequest(new DbUpdateException("保存到数据库时遇到错误"));
            }

            // 获取新地址。因为新地址 ID 是由数据库生成，所以要从数据库获取。
            var newAddresses = await _addressRepository.FindByUserIdAsync(user.Id);
            var newAddress = newAddresses.Except(oldAddresses);

            // 返回新地址
            return Ok(_mapper.Map<AddressDto>(newAddress.FirstOrDefault()));
        }

        [HttpPost("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> ChangeAddressAsync([FromBody] AddressDto addressDto)
        {
            // 参数验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // 参数验证
            if (!await _addressRepository.ExistsAsync(addressDto.Id))
            {
                return BadRequest(new ArgumentNullException("地址不存在"));
            }

            // 参数验证
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest(new ArgumentNullException("用户不存在"));
            }

            // 更新数据库
            var address = _mapper.Map<Address>(addressDto);
            _addressRepository.Update(address);
            var result = await _addressRepository.SaveAsync();
            
            return result ? NoContent() : BadRequest(new DbUpdateException("保存到数据库时遇到错误"));
        }

        [HttpPost("[Action]")]
        [Authorize(Roles = "customer, admin, manager")]
        public async Task<IActionResult> DeleteAddressAsync([FromBody] AddressDto addressDto)
        {
            var address = await _addressRepository.FindByIdAsync(addressDto.Id);
            if (address is null)
            {
                return BadRequest(new ArgumentNullException("地址不存在"));
            }

            try
            {
                _addressRepository.Remove(address);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        private async Task<bool> ExistsAsync(AddressDto addressDto)
            => await _addressRepository.FindByIdAsync(addressDto.Id) is not null;

    }
}
