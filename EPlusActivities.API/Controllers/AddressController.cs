using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
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
    public class AddressController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserIdRepository<Address> _addressRepository;
        private readonly IMapper _mapper;
        public AddressController(
            UserManager<ApplicationUser> userManager,
            IUserIdRepository<Address> addressRepository,
            IMapper mapper)
        {
            _addressRepository = addressRepository
                ?? throw new ArgumentNullException(nameof(addressRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet("list")]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetByUserIdAsync([FromBody] AddressDto addressDto)
        {
            #region 参数验证
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("用户不存在");
            }
            #endregion

            var addresses = await _addressRepository.FindByUserIdAsync(addressDto.UserId);
            return Ok(_mapper.Map<IEnumerable<AddressDto>>(addresses));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AddressDto>> GetByIdAsync([FromBody] AddressDto addressDto)
        {
            #region 参数验证
            if (!addressDto.Id.HasValue)
            {
                return BadRequest("地址 ID 不得为空");
            }
            #endregion

            var address = await _addressRepository.FindByIdAsync(addressDto.Id.Value);
            return address is null ? NotFound("地址不存在") : Ok(address);
        }

        [HttpPost]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AddressDto>> AddAddressAsync([FromBody] AddressDto addressDto)
        {
            #region 参数验证
            if (addressDto.Id.HasValue && await _addressRepository.ExistsAsync(addressDto.Id.Value))
            {
                return BadRequest("地址已存在");
            }

            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }

            var oldAddresses = await _addressRepository.FindByUserIdAsync(user.Id);
            if (oldAddresses?.Count() >= 5)
            {
                return BadRequest("用户不可添加超过 5 个地址");
            }
            #endregion

            #region 数据库操作
            var address = _mapper.Map<Address>(addressDto);
            address.Id = Guid.NewGuid();
            await _addressRepository.AddAsync(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion 

            return succeeded
                ? Ok(_mapper.Map<AddressDto>(address))
                : new InternalServerErrorObjectResult("保存到数据库时发生错误");
        }

        [HttpPut]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] AddressDto addressDto)
        {
            #region 参数验证
            if (!addressDto.Id.HasValue)
            {
                return BadRequest("地址 ID 不得为空");
            }
            
            if (!await _addressRepository.ExistsAsync(addressDto.Id.Value))
            {
                return NotFound("地址不存在");
            }

            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("用户不存在");
            }
            #endregion

            #region 数据库操作
            var address = _mapper.Map<Address>(addressDto);
            _addressRepository.Update(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded ? Ok() : new InternalServerErrorObjectResult("保存到数据库时遇到错误");
        }

        [HttpDelete]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAddressAsync([FromBody] AddressDto addressDto)
        {
            #region 参数验证
            if (!addressDto.Id.HasValue)
            {
                return BadRequest("地址 ID 不得为空");
            }
            
            var address = await _addressRepository.FindByIdAsync(addressDto.Id.Value);
            if (address is null)
            {
                return BadRequest("地址不存在");
            }
            #endregion

            #region 数据库操作
            _addressRepository.Remove(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded ? Ok() : new InternalServerErrorObjectResult("保存到数据库时遇到错误");
        }
    }
}
