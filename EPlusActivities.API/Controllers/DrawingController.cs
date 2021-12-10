using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.DrawingCommand;
using EPlusActivities.API.Application.Queries.DrawingQueries;
using EPlusActivities.API.Application.Queries.LotteryStatementQueries;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 抽奖记录 API
    /// </summary>
    [Route("choujiang/api/[controller]")]
    [ApiController]
    public class DrawingController : Controller
    {
        private readonly IMediator _mediator;

        public DrawingController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 获取某个用户的抽奖记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<DrawingDto>>> GetLotteryRecordsByUserIdAsync(
            [FromQuery] GetDrawingRecordsByUserIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取某个用户的中奖记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("winners")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<DrawingDto>>> GetWinningRecordsByUserIdAsync(
            [FromQuery] GetWinningRecordsByUserIdQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<IEnumerable<DrawingDto>>> CreateAsync(
            [FromBody] DrawCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 更新抽奖记录
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UpdateAsync(
            [FromBody] UpdateDrawingRecordCommand notification
        )
        {
            await _mediator.Publish(notification);
            return Ok();
        }

        /// <summary>
        /// 删除抽奖记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteDrawingRecordCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
