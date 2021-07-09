using System;

namespace EPlusActivities.API.DTOs
{
    public class PrizeDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Catalog { get; set; }
        public decimal UnitPrice { get; set; }
        public int RequiredCredit { get; set; }
        public string PictureUrl { get; set; }
        public Guid? LotteryId { get; set; }
    }
}