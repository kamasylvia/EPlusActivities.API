namespace EPlusActivities.API.Infrastructure.Enums
{
    /// <summary>
    /// 奖品种类：分为虚拟奖品和实体奖品。
    /// </summary>
    public enum PrizeType
    {
        Default,
        // 积分
        Credit,
        // 优惠券
        Coupon,
        // 实体
        Physical
    }
}
