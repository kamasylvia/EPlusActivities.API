using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.DrawingDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.DrawingQueries
{
    public class GetDrawingRecordsByUserIdQuery : IRequest<IEnumerable<DrawingDto>>
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
