using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.ActivityUserCommands;
using EPlusActivities.API.Application.Queries.ActivityUserQueries;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 活动和用户的绑定关系 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class ActivityUserController : Controller
    {
        private readonly IMediator _mediator;

        public ActivityUserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 获取活动和用户的绑定关系
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<ActivityUserDto>> GetByIdAsync(
            [FromQuery] GetActivityUserQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取某个用户正在参与的活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("activities")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<ActivityUserDto>>> GetByUserIdAsync(
            [FromQuery] GetActivityUserByUserIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 绑定活动和用户，该 API 必须在签到和抽奖 API 之前被调用，否则无法判断用户参与的是哪个活动。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<IActionResult> JoinAsync([FromBody] BindActivityAndUserCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 为某个用户绑定所有可参加的活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("bindAvailable")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<IEnumerable<ActivityUserDto>>> JoinAvailableActivities(
            [FromBody] JoinAvailableActivitiesCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 用户积分兑换抽奖次数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("redeeming")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<ActivityUserForRedeemDrawsResponseDto>> RedeemDrawsAsync(
            [FromBody] RedeemCommand request
        ) => Ok(await _mediator.Send(request));
    }
}
