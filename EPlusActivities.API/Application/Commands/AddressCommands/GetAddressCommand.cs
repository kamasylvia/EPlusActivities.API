using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.AddressCommands
{
    public class GetAddressCommand : IRequest<AddressDto>
    {
        /// <summary>
        /// 地址 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
