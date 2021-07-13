using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs
{
    public class PrizeDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int RequiredCredit { get; set; }
        public string PictureUrl { get; set; }
        public Guid? LotteryId { get; set; }
    }
}