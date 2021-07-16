using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.BrandDtos
{
    public class BrandForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
