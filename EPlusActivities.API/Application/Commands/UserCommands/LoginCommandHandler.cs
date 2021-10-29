using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class LoginCommandHandler : BaseCommandHandler, IRequestHandler<LoginCommand, UserDto>
    {
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IMemberService memberService
        ) : base(userManager)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        public async Task<UserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            #region Get member info
            if ((await _userManager.GetRolesAsync(user)).Contains("customer"))
            {
                var memberDto = await _memberService.GetMemberAsync(
                    user.PhoneNumber,
                    request.ChannelCode
                // Enum.Parse<ChannelCode>(request.ChannelCode, true)
                );
                user.IsMember = true;
                user.MemberId = memberDto.Body.Content.MemberId;
                user.Credit = memberDto.Body.Content.Points;
            }
            #endregion

            #region Update the user
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException();
            }
            #endregion

            return _mapper.Map<UserDto>(user);
        }
    }
}
