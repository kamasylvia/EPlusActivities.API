using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForGetUsersDto : DtoForGetList
    {
        /// <summary>
        /// 角色。大小写不敏感。
        /// </summary>
        /// <value></value>
        [Required]
        public string Role { get; set; }
    }
}
