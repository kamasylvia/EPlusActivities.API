using System.ComponentModel.DataAnnotations;
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
