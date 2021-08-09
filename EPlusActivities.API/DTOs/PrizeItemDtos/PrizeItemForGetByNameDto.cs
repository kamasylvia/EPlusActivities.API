using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.PrizeItemDtos
{
    public class PrizeItemForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
