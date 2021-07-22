using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // 奖品种类
        public Category Category { get; set; }

        // 奖品品牌
        public Brand Brand { get; set; }

        // 奖品单价
        public decimal? UnitPrice { get; set; }

        // 奖品数量
        public int Quantity { get; set; }

        // 奖品图片
        public PrizePhoto Photo { get; set; }

        // 奖品库存
        public int Stock { get; set; }

        public IEnumerable<Lottery> LotteryResults { get; set; }

        public IEnumerable<PrizeTypePrizeItem> PrizeTypePrizeItems { get; set; }
    }
}
