using System;

namespace EPlusActivities.API.DTOs
{
    public class LotteryDto
    {
        public Guid Id { get; set; }
        public string Channel { get; set; }
        public Guid WinnerId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid PrizeId { get; set; }
    }
}