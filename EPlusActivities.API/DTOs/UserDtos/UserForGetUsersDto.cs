using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForGetUsersDto
    {
        /// <summary>
        /// 角色。大小写不敏感。
        /// </summary>
        /// <value></value>
        [Required]
        public string Role { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        /// <value></value>
        public int Page { get; set; }

        /// <summary>
        /// 每页多少个
        /// </summary>
        /// <value></value>
        public int Num { get; set; }
    }
}
