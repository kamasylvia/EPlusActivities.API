using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class GetPrizeItemByIdCommand : IRequest<PrizeItemDto>
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
