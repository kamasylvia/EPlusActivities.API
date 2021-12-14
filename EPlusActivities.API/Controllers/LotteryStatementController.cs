using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClosedXML.Extensions;
using EPlusActivities.API.Application.Queries.LotteryStatementQueries;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotteryStatementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LotteryStatementController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 根据活动号和日期查询抽奖数、中奖数、兑换数
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<
            ActionResult<IEnumerable<GetLotterySummaryResponse>>
        > GetGeneralRecordsAsync([FromQuery] GetLotterySummaryStatementQuery request) =>
            Ok(await _mediator.Send(request));

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("details")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<GetLotteryDetailsResponse>> GetDetailedRecordsAsync(
            [FromQuery] GetLotteryDetailsQuery request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 管理员根据活动号下载中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("excel")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<HttpResponseMessage> DownloadLotteryExcelAsyncs(
            [FromQuery] DownloadLotteryStatementQuery request
        ) => (await _mediator.Send(request)).Deliver($"LotteryStatement-{request.StartDate}-{request.EndDate}.xlsx");
    }
}
