using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.PrizeItemDtos
{
    public class PrizeItemForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public PrizeType PrizeType { get; set; }

        // 会员系统优惠券活动码
        public string CouponActiveCode { get; set; }

        // 积分奖品
        public int? Credit { get; set; }

        public int Quantity { get; set; }

        public string CategoryName { get; set; }

        public string BrandName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int RequiredCredit { get; set; }

        public string PhotoUrl { get; set; }

        public int Stock { get; set; }
    }
}
