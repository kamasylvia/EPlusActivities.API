using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Application.Commands.LotteryCommands;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<
            ActionResult<LotteryRecordsForManagerResponse>
        > GetLotteryRecordsForManagerAsync([FromQuery] LotteryRecordsForManagerRequest request)
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var lotteries = await activity.LotteryResults
                .Where(
                    lr =>
                        lr.IsLucky
                        && Enum.Parse<ChannelCode>(request.Channel, true) == lr.ChannelCode
                        && !(request.StartTime > lr.DateTime)
                        && !(lr.DateTime > request.EndTime)
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion
            return Ok(_lotteryService.CreateLotteryForDownload(lotteries));
        }

        /// <summary>
        /// 管理员根据活动号下载中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/excel")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DownloadLotteryRecordsForManagerAsyncs(
            [FromQuery] LotteryRecordsForManagerRequest request
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var lotteries = await activity.LotteryResults
                .Where(
                    lr =>
                        lr.IsLucky
                        && Enum.Parse<ChannelCode>(request.Channel, true) == lr.ChannelCode
                        && !(request.StartTime > lr.DateTime)
                        && !(lr.DateTime > request.EndTime)
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion

            var generalLotteryRecords = await _generalLotteryRecordsRepository.FindByDateRangeAsync(
                activity.Id.Value,
                Enum.Parse<ChannelCode>(request.Channel, true),
                request.StartTime,
                request.EndTime
            );

            var (memoryString, contentType) = _lotteryService.DownloadLotteryRecords(
                generalLotteryRecords,
                lotteries
            );

            return File(memoryString, contentType);
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
        > GetGeneralStatementsAsync([FromQuery] LotteryForGetGeneralRecordsRequest request)
        {
            #region Parameter validation
            var channel = Enum.Parse<ChannelCode>(request.Channel, true);
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var generalLotteryRecords = await _generalLotteryRecordsRepository.FindByDateRangeAsync(
                activity.Id.Value,
                channel,
                request.StartTime,
                request.EndTime
            );
            #endregion

            return Ok(
                _mapper.Map<IEnumerable<LotteryForGetGeneralRecordsResponse>>(generalLotteryRecords)
            );
        }

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
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] LotteryForUpdateDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<LotteryForUpdateDto, Lottery>(lotteryDto, lottery);
            lottery.PickedUpTime = lotteryDto.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            if (succeeded)
            {
                return Ok();
            }

            _logger.LogError("Failed to update the lottery");
            return new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 删除抽奖记录
        /// </summary>
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] LotteryForGetByIdDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            _lotteryRepository.Remove(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            if (succeeded)
            {
                return Ok();
            }
            _logger.LogError("Failed to delete the lottery");
            return new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
