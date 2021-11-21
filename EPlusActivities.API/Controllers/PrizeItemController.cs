using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.PrizeItemCommands;
using EPlusActivities.API.Application.Queries.PrizeItemQueries;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 奖品 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class PrizeItemController : Controller
    {
        private readonly IMediator _mediator;

        public PrizeItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 通过奖品名获取奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetByNameAsync(
            [FromQuery] GetPrizeItemByNameQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 通过奖品 ID 列表获取奖品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("group")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeItemDto>> GetByIdsAsync(
            [FromQuery] GetPrizeItemGroupQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 通过奖品 ID 获取奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeItemDto>> GetByIdAsync(
            [FromQuery] GetPrizeItemByIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取奖品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetItemListAsync(
            [FromQuery] GetPrizeItemListQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 新建奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<PrizeItemDto>> CreateAsync(
            [FromBody] CreatePrizeItemCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 修改奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdatePrizeItemCommand request) =>
            Ok(await _mediator.Send(request));

        /// <summary>
        /// 删除奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeletePrizeItemCommand request) =>
            Ok(await _mediator.Send(request));
    }
}
