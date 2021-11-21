using System.Threading;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class UpdatePhoneCommandHandler
        : UserRequestHandlerBase,
          IRequestHandler<UpdatePhoneCommand>
    {
        public UpdatePhoneCommandHandler(UserManager<ApplicationUser> userManager)
            : base(userManager) { }

        public async Task<Unit> Handle(
            UpdatePhoneCommand request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user is null)
            {
                throw new NotFoundException("Could not find the user.");
            }

            if (user.PhoneNumber == request.PhoneNumber)
            {
                throw new ConflictException("The new phone number is the same as the old one.");
            }
            #endregion

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(
                user,
                request.PhoneNumber
            );
            var result = await _userManager.ChangePhoneNumberAsync(
                user,
                request.PhoneNumber,
                token
            );
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }

            result = await _userManager.SetUserNameAsync(user, request.PhoneNumber);
            if (!result.Succeeded)
            {
                throw new DatabaseUpdateException(result.ToString());
            }

            return Unit.Value;
        }
    }
}
