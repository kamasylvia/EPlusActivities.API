using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Dtos.LotteryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Queries.LotteryQueries
{
    public class GetWinningRecordsByUserIdQuery : IRequest<IEnumerable<LotteryDto>>
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
