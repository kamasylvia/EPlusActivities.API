using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Extensions
{
    public static class PrizeTypeExtensions
    {
        public static string ToChinese(this PrizeType prizeType)
        {
            switch (prizeType)
            {
                case PrizeType.None:
                    return "无奖";
                case PrizeType.Coupon:
                    return "优惠券";
                case PrizeType.Credit:
                    return "积分";
                case PrizeType.Physical:
                    return "实物";
                default:
                    return "无奖";
            }
        }
    }
}
