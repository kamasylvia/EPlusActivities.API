﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.ActivityDtos;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.ActivityCommands
{
    public class CreateActivityCommand : IRequest<ActivityDto>
    {
        /// <summary>
        /// 活动名
        /// </summary>
        /// <value></value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 全活动周期抽奖次数上限，null 表示无限。
        /// </summary>
        /// <value></value>
        public int? Limit { get; set; }

        /// <summary>
        /// 每日抽奖次数上限，null 表示无限。
        /// </summary>
        /// <value></value>
        public int? DailyDrawLimit { get; set; }

        /// <summary>
        /// 每日兑换次数上限，null 表示无限。
        /// </summary>
        /// <value></value>
        public int? DailyRedemptionLimit { get; set; }

        /// <summary>
        /// 兑换一次抽奖所需积分，null 表示非抽奖活动
        /// </summary>
        /// <value></value>
        public int? RequiredCreditForRedeeming { get; set; }

        /// <summary>
        /// 允许参加该活动的渠道，字符串不区分大小写。
        /// 取值范围：Ngs, NgsPlaza, Alldays
        /// </summary>
        /// <value></value>
        [Required]
        public IEnumerable<string> AvailableChannels { get; set; }

        /// <summary>
        /// 活动展示类型，字符串不区分大小写。
        /// 取值范围：None, Roulette, Digging, Scratchcard
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(LotteryDisplay))]
        public LotteryDisplay LotteryDisplay { get; set; }

        /// <summary>
        /// 活动类型，字符串不区分大小写。
        /// 取值范围：Default, SingleAttendance, SequentialAttendance, Lottery
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(ActivityType))]
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 活动结束时间，不传或传 null 表示至今。
        /// </summary>
        /// <value></value>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        /// <value></value>
        [RegularExpression(
            "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$",
            ErrorMessage = "Invalid color format"
        )]
        public string Color { get; set; }
    }
}
