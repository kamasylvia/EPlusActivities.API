using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class UpdatePhoneCommandHandler : BaseCommandHandler, IRequestHandler<UpdatePhoneCommand>
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
