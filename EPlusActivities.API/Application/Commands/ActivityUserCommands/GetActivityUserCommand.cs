using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityUserCommands
{
    public class GetActivityUserCommand : IRequest<ActivityUserDto>
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
