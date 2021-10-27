using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class GetCategoryByContainedNameCommand : IRequest<CategoryDto>
    {
        /// <summary>
        /// 分类名称中包含的关键字
        /// </summary>
        /// <value></value>
        [Required]
        public string Keyword { get; set; }
    }
}
