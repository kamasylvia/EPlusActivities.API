using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.CategoryQueries
{
    public class GetCategoryByContainedNameQuery : IRequest<CategoryDto>
    {
        /// <summary>
        /// 分类名称中包含的关键字
        /// </summary>
        /// <value></value>
        [Required]
        public string Keyword { get; set; }
    }
}
