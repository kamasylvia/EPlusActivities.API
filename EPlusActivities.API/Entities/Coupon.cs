using System;

namespace EPlusActivities.API.Entities
{
    public class Coupon
    {
        public Guid? Id { get; set; }

        public string Code { get; set; }

        // 这个属性在签到抽奖中是无效的，留给将来版本
        public bool Used { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual PrizeItem PrizeItem { get; set; }
    }
}
