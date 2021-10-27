using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.CategoryCommands
{
    public class UpdateCategoryNameCommand : IRequest
    {
        /// <summary>
        /// 分类 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 新名称
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }
    }
}
