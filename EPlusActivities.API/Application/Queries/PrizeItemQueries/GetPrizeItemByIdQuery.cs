using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.PrizeItemDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.PrizeItemQueries
{
    public class GetPrizeItemByIdQuery : IRequest<PrizeItemDto>
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
