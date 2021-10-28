using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class DeletePrizeItemCommand : IRequest
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
