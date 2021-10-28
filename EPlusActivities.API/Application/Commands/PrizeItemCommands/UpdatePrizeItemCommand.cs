using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace EPlusActivities.API.Application.Commands.PrizeItemCommands
{
    public class UpdatePrizeItemCommand : IRequest
    {
        /// <summary>
        /// 奖品 ID
        /// </summary>
        /// <value></value>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// 奖品名
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 奖品类型，可选：Default, Credit, Coupon, Physical
        /// </summary>
        /// <value></value>
        public string PrizeType { get; set; }

        /// <summary>
        /// 会员系统优惠券活动码
        /// </summary>
        /// <value></value>
        public string CouponActiveCode { get; set; }

        /// <summary>
        /// 积分奖品，null 表示非积分奖品
        /// </summary>
        /// <value></value>
        public int? Credit { get; set; }

        /// <summary>
        /// 奖品数量
        /// </summary>
        /// <value></value>
        public int Quantity { get; set; }

        /// <summary>
        /// 奖品种类
        /// </summary>
        /// <value></value>
        public string CategoryName { get; set; }

        /// <summary>
        /// 奖品品牌
        /// </summary>
        /// <value></value>
        public string BrandName { get; set; }

        /// <summary>
        /// 奖品单价
        /// </summary>
        /// <value></value>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 所需积分
        /// </summary>
        /// <value></value>
        public int RequiredCredit { get; set; }

        /// <summary>
        /// 奖品库存
        /// </summary>
        /// <value></value>
        public int? Stock { get; set; }
    }
}
