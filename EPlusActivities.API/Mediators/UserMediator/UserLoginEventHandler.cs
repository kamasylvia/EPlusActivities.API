using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Mediators.UserMediator
{
    public class UserLoginEventHandler : IRequestHandler<UserLoginEvent, UserDto>
    {
        private readonly IMediator _mediator;
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserLoginEventHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLoginEventHandler(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IMemberService memberService,
            IMapper mapper,
            ILogger<UserLoginEventHandler> logger
        )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> Handle(
            UserLoginEvent request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                return null;

            if ((await _userManager.GetRolesAsync(user)).Contains("customer"))
            {
                var (getMemberSucceed, memberDto) = await _memberService.GetMemberAsync(
                    user.PhoneNumber
                );
                if (getMemberSucceed)
                {
                    user.IsMember = true;
                    user.MemberId = memberDto.Body.Content.MemberId;
                    user.Credit = memberDto.Body.Content.Points;
                }
                else
                {
                    _logger.LogError($"UserLoginEventHandler: 获取会员信息失败：{memberDto.Header.Message}");
                    throw new NotImplementedException();
                }
            }

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                _logger.LogError($"UserLoginEventHandler: 更新用户信息失败：{updateUserResult.ToString()}");
                throw new NotImplementedException();
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
