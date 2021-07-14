using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs
{
    public class PrizeDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string BrandName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int RequiredCredit { get; set; }

        public string PictureUrl { get; set; }

        public Guid LotteryId { get; set; }
    }
}
