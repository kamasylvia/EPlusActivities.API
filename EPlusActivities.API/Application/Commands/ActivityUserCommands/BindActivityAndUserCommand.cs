using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class BindActivityAndUserCommand : IRequest
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
