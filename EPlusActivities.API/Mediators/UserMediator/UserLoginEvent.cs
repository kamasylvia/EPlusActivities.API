using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.UserDtos;
using MediatR;

namespace EPlusActivities.API.Mediators.UserMediator
{
    public class UserLoginEvent : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
