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
        private readonly IFindByUserIdRepository<Address> _addressRepository;
        private readonly IMapper _mapper;
        public AddressController(
            UserManager<ApplicationUser> userManager,
            IFindByUserIdRepository<Address> addressRepository,
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
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }
            #endregion

            var addresses = await _addressRepository.FindByUserIdAsync(addressDto.UserId);
            return Ok(_mapper.Map<IEnumerable<AddressDto>>(addresses));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AddressDto>> GetByIdAsync([FromBody] AddressDto addressDto)
        {
            #region Parameter validation
            if (addressDto.Id == Guid.Empty)
            {
                return BadRequest("The ID could not be null.");
            }
            #endregion

            var address = await _addressRepository.FindByIdAsync(addressDto.Id);
            return address is null ? NotFound($"Could not find the address with ID '{addressDto.Id}'.") : Ok(address);
        }

        [HttpPost]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<AddressDto>> CreateAsync([FromBody] AddressDto addressDto)
        {
            #region Parameter validation
            if (await _addressRepository.ExistsAsync(addressDto.Id))
            {
                return Conflict("This address is already existed.");
            }

            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var oldAddresses = await _addressRepository.FindByUserIdAsync(user.Id);
            if (oldAddresses?.Count() >= 5)
            {
                return BadRequest("Could not add more than 5 addresses.");
            }
            #endregion

            #region Database operations
            var address = _mapper.Map<Address>(addressDto);
            address.Id = Guid.NewGuid();
            await _addressRepository.AddAsync(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion 

            return succeeded
                ? Ok(_mapper.Map<AddressDto>(address))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] AddressDto addressDto)
        {
            #region Parameter validation
            if (addressDto.Id == Guid.Empty)
            {
                return BadRequest("The ID could not be null.");
            }

            if (!await _addressRepository.ExistsAsync(addressDto.Id))
            {
                return NotFound("Could not find the address.");
            }

            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            #region Database operations
            var address = _mapper.Map<Address>(addressDto);
            _addressRepository.Update(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception.");
        }

        [HttpDelete]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] AddressDto addressDto)
        {
            #region Parameter validation
            if (addressDto.Id == Guid.Empty)
            {
                return BadRequest("The ID could not be null.");
            }

            var address = await _addressRepository.FindByIdAsync(addressDto.Id);
            if (address is null)
            {
                return NotFound("Could not find the address.");
            }
            #endregion

            #region Database operations
            _addressRepository.Remove(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception.");
        }
    }
}
