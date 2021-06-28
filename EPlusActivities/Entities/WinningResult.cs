using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.Entities
{
    public class WinningResult
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string WinnerId { get; set; }
        public ApplicationUser Winner { get; set; }

        [Required]
        public string PrizeId { get; set; }
        public Prize PrizeItem { get; set; }

        [Required]
        public string ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}