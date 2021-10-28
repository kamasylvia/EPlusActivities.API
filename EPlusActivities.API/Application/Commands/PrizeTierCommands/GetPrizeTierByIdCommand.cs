using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class GetPrizeTierByIdCommand : IRequest<PrizeTierDto>
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
