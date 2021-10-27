using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class CreateBrandCommand : IRequest<BrandDto>
    {
        [Required]
        public string Name { get; set; }
    }
}
