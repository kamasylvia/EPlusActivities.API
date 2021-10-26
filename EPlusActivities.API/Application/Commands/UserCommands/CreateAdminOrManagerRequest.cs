using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class CreateAdminOrManagerCommand : IRequest
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
