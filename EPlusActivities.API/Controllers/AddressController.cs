using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.AddressDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IFindByParentIdRepository<Address> _addressRepository;
        private readonly IMapper _mapper;
        public AddressController(
            UserManager<ApplicationUser> userManager,
            IFindByParentIdRepository<Address> addressRepository,
            IMapper mapper
        ) {
            _addressRepository =
                addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet("user")]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetByUserIdAsync(
            [FromQuery] AddressForGetByUserIdDto addressDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return BadRequest("Could not find the user.");
            }
            #endregion

            var addresses = await _addressRepository.FindByParentIdAsync(addressDto.UserId.Value);
            return addresses.Count() > 0
                ? Ok(_mapper.Map<IEnumerable<AddressDto>>(addresses))
                : NotFound(
                        $"Could not find any addresses with the specified user '{addressDto.UserId.Value}'"
                    );
        }

        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<AddressDto>> GetByIdAsync(
            [FromQuery] AddressForGetByIdDto addressDto
        ) {
            var address = await _addressRepository.FindByIdAsync(addressDto.Id.Value);
            return address is null
                ? NotFound($"Could not find the address with ID '{addressDto.Id}'.")
                : Ok(address);
        }

        [HttpPost]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<AddressDto>> CreateAsync(
            [FromBody] AddressForCreateDto addressDto
        ) {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(addressDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var oldAddresses = await _addressRepository.FindByParentIdAsync(user.Id);
            if (oldAddresses?.Count() >= 5)
            {
                return BadRequest("Could not add more than 5 addresses.");
            }
            #endregion

            #region Database operations
            if (addressDto.IsDefault)
            {
                var addresses = await _addressRepository.FindByParentIdAsync(
                    addressDto.UserId.Value
                );
                if (addresses.Count() > 0)
                {
                    var oldDefaultAddress = addresses.Single(x => x.IsDefault);
                    oldDefaultAddress.IsDefault = false;
                    _addressRepository.Update(oldDefaultAddress);
                }
            }

            var address = _mapper.Map<Address>(addressDto);
            await _addressRepository.AddAsync(address);
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<AddressDto>(address))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] AddressForUpdateDto addressDto)
        {
            var address = await _addressRepository.FindByIdAsync(addressDto.Id.Value);

            #region Parameter validation
            if (address is null)
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
            if (addressDto.IsDefault)
            {
                var addresses = await _addressRepository.FindByParentIdAsync(
                    addressDto.UserId.Value
                );
                if (addresses.Count() > 0)
                {
                    var oldDefaultAddress = addresses.Single(x => x.IsDefault);
                    oldDefaultAddress.IsDefault = false;
                    _addressRepository.Update(oldDefaultAddress);
                }
            }

            _addressRepository.Update(
                _mapper.Map<AddressForUpdateDto, Address>(addressDto, address)
            );
            var succeeded = await _addressRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception.");
        }

        [HttpDelete]
        // [Authorize(Roles = "customer, admin, manager")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] AddressForGetByIdDto addressDto)
        {
            var address = await _addressRepository.FindByIdAsync(addressDto.Id.Value);

            #region Parameter validation
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
