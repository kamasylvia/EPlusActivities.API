using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.PrizeItemDtos
{
    public class PrizeItemForCreateDto
    {
        [Required]
        public string Name { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string BrandName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int RequiredCredit { get; set; }

        public string PhotoUrl { get; set; }

        public int Stock { get; set; }
    }
}
