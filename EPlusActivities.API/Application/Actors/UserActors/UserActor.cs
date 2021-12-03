using System;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using EPlusActivities.API.Application.Commands.UserCommands;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Actors.UserActors
{
    public class UserActor : Actor, IUserActor
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserActor(ActorHost host, UserManager<ApplicationUser> userManager) : base(host)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
