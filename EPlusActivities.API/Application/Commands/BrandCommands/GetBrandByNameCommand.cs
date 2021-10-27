using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.BrandCommands
{
    public class GetBrandByNameCommand : IRequest<BrandDto>
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }
    }
}
