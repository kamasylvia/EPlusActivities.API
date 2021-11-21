using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.ActivityUserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.ActivityUserQueries
{
    public class GetActivityUserQuery : IRequest<ActivityUserDto>
    {
        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }
    }
}
