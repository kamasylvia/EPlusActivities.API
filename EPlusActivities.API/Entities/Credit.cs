using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class Credit
    {
        [Key]
        public Guid? Id { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        // 农工商会员ID
        [Required]
        public string MemberId { get; set; }

        // 更新积分值
        public int Points { get; set; }

        // 更新前积分值
        public int OldPoints { get; set; }

        // 更新后积分值
        public int NewPoints { get; set; }

        // 更新积分理由
        public string Reason { get; set; }

        // 交易流水
        public string SheetId { get; set; }

        // 积分变更流水
        public string RecordId { get; set; }

        // 更新积分类型，1-加，2-减
        public CreditUpdateType UpdateType { get; set; }
    }
}
