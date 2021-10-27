using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.CategoryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class GetCategoryByNameCommand : IRequest<CategoryDto>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }
    }
}
