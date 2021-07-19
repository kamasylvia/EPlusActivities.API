using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeType
    {
        [Key]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int Percentage { get; set; }

        [Required]
        public Activity Activity { get; set; }

        public PrizeType(string name)
        {
            this.Name = name;
        }
    }
}