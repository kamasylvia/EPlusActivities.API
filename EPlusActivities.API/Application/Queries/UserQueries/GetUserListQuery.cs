using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Dtos.UserDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.UserQueries
{
    public class GetUserListQuery : DtoForGetList, IRequest<IEnumerable<UserDto>>
    {
        /// <summary>
        /// 角色。大小写不敏感。
        /// </summary>
        /// <value></value>
        [Required]
        public string Role { get; set; }
    }
}
