using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.AddressQueries
{
    public class GetAddressListByUserIdQuery : IRequest<IEnumerable<AddressDto>>
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
