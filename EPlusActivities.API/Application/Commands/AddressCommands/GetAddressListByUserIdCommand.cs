using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class GetAddressListByUserIdCommand : IRequest<IEnumerable<AddressDto>>
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
