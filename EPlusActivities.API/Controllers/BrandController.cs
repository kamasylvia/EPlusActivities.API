using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.BrandCommands;
using EPlusActivities.API.Application.Queries.BrandQueries;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 品牌 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class BrandController : Controller
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 根据 ID 获取品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<BrandDto>> GetByIdAsync(
            [FromQuery] GetBrandByIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 根据品牌名获取品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<BrandDto>> GetByNameAsync(
            [FromQuery] GetBrandByNameQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrandListAsync(
            [FromQuery] GetBrandListQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 根据关键字查找品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetByContainedNameAsync(
            [FromBody] GetBrandByContainedNameQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<BrandDto>> CreateAsync(
            [FromBody] CreateBrandCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 修改品牌名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateNameAsync([FromBody] UpdateBrandNameCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteBrandCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
