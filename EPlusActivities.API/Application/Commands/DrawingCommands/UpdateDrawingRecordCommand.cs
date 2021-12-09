using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public record UpdateDrawingRecordCommand : INotification
    {
        /// <summary>
        /// 抽奖记录 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 是否已领取
        /// </summary>
        /// <value></value>
        public bool PickedUp { get; set; }

        /// <summary>
        /// 是否发放
        /// </summary>
        /// <value></value>
        public bool Delivered { get; set; }

        /// <summary>
        /// 领取日期
        /// </summary>
        /// <value></value>
        public DateTime? PickedUpTime { get; set; }
    }
}
