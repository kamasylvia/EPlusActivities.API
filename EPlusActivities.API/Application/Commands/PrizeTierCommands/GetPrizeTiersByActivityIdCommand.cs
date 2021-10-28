using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.PrizeTierDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeTierCommands
{
    public class GetPrizeTiersByActivityIdCommand:IRequest<IEnumerable<PrizeTierDto>>
    {
        [Required]
        public Guid? ActivityId { get; set; }
    }
}
