using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.UserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class GetUserListCommand : DtoForGetList, IRequest<IEnumerable<UserDto>>
    {
        /// <summary>
        /// 角色。大小写不敏感。
        /// </summary>
        /// <value></value>
        [Required]
        public string Role { get; set; }
    }
}
