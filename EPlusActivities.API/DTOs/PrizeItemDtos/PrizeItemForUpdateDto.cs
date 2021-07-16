using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.DTOs.PrizeItemDtos
{
    public class PrizeItemForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string CategoryName { get; set; }

        public string BrandName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int RequiredCredit { get; set; }

        public string PictureUrl { get; set; }

        public Guid? LotteryId { get; set; }
    }
}
