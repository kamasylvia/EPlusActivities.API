using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class GetUserListCommandHandler
        : BaseHandler,
          IRequestHandler<GetUserListCommand, IEnumerable<UserResponse>>
    {
        private readonly IMapper _mapper;
        public GetUserListCommandHandler(IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<UserResponse>> Handle(
            GetUserListCommand request,
            CancellationToken cancellationToken
        )
        {
            var userList = (
                await _userManager.GetUsersInRoleAsync(request.Role.Trim().ToLower())
            ).ToList();

            var startIndex = (request.PageIndex - 1) * request.PageSize;
            var count = request.PageIndex * request.PageSize;

            if (userList.Count < startIndex)
            {
                throw new NotFoundException("Could not find any user.");
            }

            if (userList.Count - startIndex < count)
            {
                count = userList.Count - startIndex;
            }

            var result = userList.GetRange(startIndex, count);
            if (result.Count <= 0)
            {
                throw new NotFoundException("Could not find any user.");
            }

            return _mapper.Map<IEnumerable<UserResponse>>(result);
        }
    }
}
