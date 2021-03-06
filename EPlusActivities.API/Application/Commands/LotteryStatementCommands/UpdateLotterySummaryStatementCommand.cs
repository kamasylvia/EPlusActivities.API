using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryStatementCommands
{
    public record UpdateLotterySummaryStatementCommand : INotification
    {
        public Guid ActivityId { get; set; }

        public ChannelCode Channel { get; set; }

        public DateTime Date { get; set; }

        public int Draws { get; set; }

        public int Winners { get; set; }

        public int Redemption { get; set; }
    }
}
