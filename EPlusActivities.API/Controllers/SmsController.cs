using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.SmsCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 短信验证码 API
    /// </summary>
    [ApiController]
    [Route("choujiang/api/[controller]")]
    public class SmsController : Controller
    {
        private readonly IMediator _mediator;

        public SmsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetVerificationCodeAsync([FromBody] GetVerificationCodeCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
