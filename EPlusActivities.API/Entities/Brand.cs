using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class Brand
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Prize> Prizes{ get; set; }

        public Brand(string name)
        {
            Name = name;
        }
    }
}