using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Enums;
using MediatR;

namespace EPlusActivities.API.Application.Commands.DrawingCommand
{
    public record DownloadLotteryStatementExcelCommand : IRequest<FileDto>
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        /// <value></value>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        /// <value></value>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        /// <value></value>
        [Required]
        [EnumDataType(typeof(ChannelCode))]
        public ChannelCode Channel { get; set; }

        /// <summary>
        /// 活动号
        /// </summary>
        /// <value></value>
        [Required]
        public string ActivityCode { get; set; }
    }

    public class FileDto
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
    }
}
