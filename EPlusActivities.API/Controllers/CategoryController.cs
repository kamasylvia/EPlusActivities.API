using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Application.Commands.CategoryCommands;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.CategoryDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 根据 ID 获取分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<CategoryDto>> GetByIdAsync(
            [FromQuery] GetCategoryByIdCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 根据名称获取分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<CategoryDto>> GetByNameAsync(
            [FromQuery] GetCategoryByNameCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 根据关键字查找分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByContainedNameAsync(
            [FromQuery] GetCategoryByContainedNameCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取奖品分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoryListAsync(
            [FromQuery] GetCategoryListCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 创建奖品分类
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<CategoryDto>> CreateAsync(
            [FromBody] CreateCategoryCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 修改分类名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("name")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateNameAsync(
            [FromBody] UpdateCategoryNameCommand request
        )
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除奖品分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteCategoryCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
