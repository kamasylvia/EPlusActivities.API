using System;

namespace EPlusActivities.API.DTOs
{
    public class LotteryDto
    {
        public Guid Id { get; set; }

        public string Channel { get; set; }

        public bool IsLucky { get; set; }

        public int UsedCredit { get; set; }

        public Guid UserId { get; set; }

        public Guid ActivityId { get; set; }

        public Guid PrizeId { get; set; }
    }
}
