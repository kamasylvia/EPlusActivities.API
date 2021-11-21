using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeTierQueries
{
    public class GetPrizeTierByIdQuery : IRequest<PrizeTierDto>
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
