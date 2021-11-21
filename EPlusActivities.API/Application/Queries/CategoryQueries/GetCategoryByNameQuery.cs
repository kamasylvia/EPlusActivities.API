using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.CategoryQueries
{
    public class GetCategoryByNameQuery : IRequest<CategoryDto>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }
    }
}
