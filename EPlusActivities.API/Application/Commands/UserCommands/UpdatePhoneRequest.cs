using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.UserCommands
{
    public class UpdatePhoneCommand : IRequest
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        [Phone]
        [StringLength(
            11,
            MinimumLength = 11,
            ErrorMessage = "A valid phone number must be 11 digits."
        )]
        public string PhoneNumber { get; set; }
    }
}
