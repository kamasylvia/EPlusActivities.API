using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class GetUserCommand : IRequest<UserDto>
    {
        [Required]
        public Guid? UserId { get; set; }

        public ChannelCode ChannelCode { get; set; }
    }
}
