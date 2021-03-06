using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class PrizeItem
    {
        public PrizeItem(string name)
        {
            Name = name;
        }

        [Key]
        public Guid? Id { get; set; }

        // 奖品名称
        [Required]
        public string Name { get; set; }

        public PrizeType PrizeType { get; set; }

        // 会员系统优惠券活动码
        public string CouponActiveCode { get; set; }

        // 奖品种类
        public virtual Category Category { get; set; }

        // 奖品品牌
        public virtual Brand Brand { get; set; }

        // 奖品单价
        public decimal? UnitPrice { get; set; }

        // 积分奖品
        public int? Credit { get; set; }

        // 奖品数量
        public int Quantity { get; set; }

        // 奖品库存
        public int? Stock { get; set; }

        public virtual IEnumerable<LotteryDetail> LotteryResults { get; set; }

        public virtual IEnumerable<PrizeTierPrizeItem> PrizeTierPrizeItems { get; set; }

        public virtual IEnumerable<Coupon> Coupons { get; set; }
    }
}
