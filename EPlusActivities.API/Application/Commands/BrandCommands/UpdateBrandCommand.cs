using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class UpdateBrandCommand : IRequest
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
