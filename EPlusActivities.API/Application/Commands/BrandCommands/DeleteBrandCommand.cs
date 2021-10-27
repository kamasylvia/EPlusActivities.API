using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class DeleteBrandCommand : IRequest
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
