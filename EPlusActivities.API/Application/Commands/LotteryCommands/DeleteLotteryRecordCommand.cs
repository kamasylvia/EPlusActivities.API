using System;
using System.ComponentModel.DataAnnotations;
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
