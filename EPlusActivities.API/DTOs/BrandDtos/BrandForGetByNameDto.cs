using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Dtos.BrandDtos
{
    public class BrandForGetByNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
