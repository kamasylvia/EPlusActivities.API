using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 用户账户 API
    /// </summary>
    [ApiController]
    [CustomActionFilterAttribute]
    [Route("choujiang/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> logger;

        public UserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<UserResponse>> GetAsync(
            [FromQuery] GetUserCommand request
        ) => Ok(await _mediator.Send<UserResponse>(request));

        /// <summary>
        /// 获取用户列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsersAsync(
            [FromQuery] GetUserListCommand request
        ) => Ok(await _mediator.Send<IEnumerable<UserResponse>>(request));

        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("phonenumber")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<IActionResult> UpdatePhoneNumberAsync(
            [FromBody] UpdatePhoneCommand request
        )
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 新建管理员账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> CreateAdminOrManagerAsync(
            [FromBody] CreateAdminOrManagerCommand request
        )
        {
            await _mediator.Send(request);
            return Ok();
        }

        /// <summary>
        /// 删除账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteUserCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
