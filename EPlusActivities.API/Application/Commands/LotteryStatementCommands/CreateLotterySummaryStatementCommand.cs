using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryStatementCommands
{
    public record CreateLotterySummaryStatementCommand : INotification
    {
        public Guid ActivityId { get; set; }

        public ChannelCode Channel { get; set; }

        public DateTime Date { get; set; }
    }
}
