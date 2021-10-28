using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.PrizeItemCommands;
using EPlusActivities.API.Application.Commands.PrizeTierCommands;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 奖品档次 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class PrizeTierController : Controller
    {
        private readonly IMediator _mediator;

        public PrizeTierController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 获取奖品档次
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeTierDto>> GetByIdAsync(
            [FromQuery] GetPrizeItemByIdCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 获取某个活动的所有奖品档次
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("activity")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<PrizeTierDto>> GetByActivityIdAsync(
            [FromQuery] GetPrizeTiersByActivityIdCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 新建奖品档次
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<PrizeTierDto>> CreateAsync(
            [FromBody] CreatePrizeTierCommand request
        ) => Ok(await _mediator.Send(request));

        /// <summary>
        /// 修改奖品档次
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdatePrizeTierCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除奖品档次，删除后其所属的活动会自动更新奖品数量。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeletePrizeTierCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
