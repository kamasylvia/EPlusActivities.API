using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.LotteryDtos;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class GetLotteryRecordsByUserIdCommand : IRequest<IEnumerable<LotteryDto>>
    {
        [Required]
        public Guid? UserId { get; set; }
    }
}
