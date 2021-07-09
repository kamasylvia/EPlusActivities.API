using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Prize
    {
        [Key]
        public Guid Id { get; set; }

        // 奖品名称
        public string Name { get; set; }

        // 奖品数量
        public int Quantity { get; set; }
        
        // 奖品种类
        public string Catalog { get; set; }

        // 奖品单价
        public decimal UnitPrice { get; set; }

        // 奖品所需积分
        public int RequiredCredit { get; set; }

        // 奖品图片 URL
        public string PictureUrl { get; set; }

        // 所处的中奖结果
        public Guid? LotteryId { get; set; }
        public Lottery Lottery { get; set; }
    }
}