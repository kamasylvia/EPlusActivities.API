using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.AddressDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.AddressQueries
{
    public class GetAddressQuery : IRequest<AddressDto>
    {
        /// <summary>
        /// 地址 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }
    }
}
