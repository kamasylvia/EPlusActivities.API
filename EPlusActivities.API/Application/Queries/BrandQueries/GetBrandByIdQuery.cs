using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.BrandQueries
{
    public class GetBrandByIdQuery : IRequest<BrandDto>
    {
        /// <summary>
        /// 品牌 Id
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
