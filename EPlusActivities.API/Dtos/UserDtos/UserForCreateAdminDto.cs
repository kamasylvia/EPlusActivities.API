using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.UserDtos
{
    public class UserForCreateAdminDto
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(32)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
