using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.ActivityCommands;
using EPlusActivities.API.Dtos.ActivityDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 活动 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IMediator _mediator;
        public ActivityController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityDto>> GetAsync(
            [FromQuery] GetActivityCommand request
        ) => await _mediator.Send(request);

        /// <summary>
        /// 根据活动号获取活动信息
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpGet("code")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityDto>> GetByActivityCodeAsync(
            [FromQuery] GetActivityByCodeCommand request
        ) => await _mediator.Send(request);

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <param name="activityDto"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivityListAsync(
            [FromQuery] GetActivityListCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 创建活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<ActivityDto>> CreateAsync(
            [FromBody] CreateActivityCommand request
        ) => await _mediator.Send(request);

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateActivityCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteActivityCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
