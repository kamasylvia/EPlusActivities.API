using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeTierQueries
{
    public class GetPrizeTiersByActivityIdQuery : IRequest<IEnumerable<PrizeTierDto>>
    {
        [Required]
        public Guid? ActivityId { get; set; }
    }
}
