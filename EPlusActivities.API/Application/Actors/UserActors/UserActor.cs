using System;
using System.Threading.Tasks;
using AutoMapper;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Application.Queries.UserQueries;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.UserActors
{
    public class UserActor : Actor, IUserActor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;

        public UserActor(
            ActorHost host,
            UserManager<ApplicationUser> userManager,
            IMemberService memberService,
            IMapper mapper
        ) : base(host)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserDto> Login(LoginQuery request)
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

        public async Task CreateAdminOrManager(CreateAdminOrManagerCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByNameAsync(command.UserName);
            if (user is not null)
            {
                throw new ConflictException("The user is already existed.");
            }
            #endregion

            user = new ApplicationUser { UserName = command.UserName };
            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
            result = await _userManager.AddToRoleAsync(user, command.Role);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
        }

        public async Task DeleteUser(DeleteUserCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.Id.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }
            #endregion

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            var oldUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
        }

        public async Task UpdatePhone(UpdatePhoneCommand command)
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(command.Id.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            if (user.PhoneNumber == command.PhoneNumber)
            {
                throw new ConflictException("The new phone number is the same as the old one.");
            }
            #endregion

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                command.PhoneNumber
            );
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                command.PhoneNumber,
                token
            );
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }

            result = await _userManager.SetUserNameAsync(user, command.PhoneNumber);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }
        }
    }
}
