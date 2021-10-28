﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.LotteryCommands;
using EPlusActivities.API.Dtos.LotteryDtos;
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
    public class LotteryController : Controller
    {
        private readonly IMediator _mediator;

        public LotteryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 获取某个用户的抽奖记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("customer/lottery-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetLotteryRecordsByUserIdAsync(
            [FromQuery] GetLotteryRecordsByUserIdCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取某个用户的中奖记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("customer/lucky-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetWinningRecordsByUserIdAsync(
            [FromQuery] GetWinningRecordsByUserIdCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/detailed-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<LotteryRecordsForManagerResponse>> GetDetailedRecordsAsync(
            [FromQuery] GetDetailedRecordsCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 管理员根据活动号下载中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/excel")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DownloadLotteryExcelAsyncs(
            [FromQuery] DownloadLotteryExcelCommand request
        )
        {
            var file = await _mediator.Send(request);
            return File(file.FileStream, file.ContentType);
        }

        /// <summary>
        /// 根据活动号和日期查询抽奖数、中奖数、兑换数
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/general-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<
            ActionResult<IEnumerable<LotteryForGetGeneralRecordsResponse>>
        > GetGeneralRecordsAsync([FromQuery] GetGeneralRecordsCommand request) =>
            Ok(await _mediator.Send(request));

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
        public async Task<ActionResult<IEnumerable<LotteryDto>>> CreateAsync(
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
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateLotteryRecordCommand notification)
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
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteLotteryRecordCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
