using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.AddressCommands;
using EPlusActivities.API.Application.Queries.AddressQueries;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 地址管理 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 获得指定用户的所有地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetByUserIdAsync(
            [FromQuery] GetAddressListByUserIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取地址信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<AddressDto>> GetByIdAsync(
            [FromQuery] GetAddressQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 新建地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            // Roles = "customer, tester"
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<AddressDto>> CreateAsync(
            [FromBody] CreateAddressCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateAddressCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteAddressCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
