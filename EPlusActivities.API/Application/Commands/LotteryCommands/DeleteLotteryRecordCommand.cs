using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace EPlusActivities.API.Application.Commands.LotteryCommands
{
    public class DeleteLotteryRecordCommand : IRequest
    {
        /// <summary>
        /// 抽奖记录 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

    }
}
