using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.UserDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Queries.UserQueries
{
    public class LoginQuery : IRequest<UserDto>, INotification
    {
        [Required]
        public Guid? UserId { get; set; }

        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode ChannelCode { get; set; }
    }
}
