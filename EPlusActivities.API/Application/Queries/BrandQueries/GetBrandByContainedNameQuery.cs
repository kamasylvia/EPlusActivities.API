using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.BrandDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.BrandQueries
{
    public class GetBrandByContainedNameQuery : IRequest<IEnumerable<BrandDto>>
    {
        /// <summary>
        /// 品牌名称中包含的关键字
        /// </summary>
        /// <value></value>
        [Required]
        public string Keyword { get; set; }
    }
}
