using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizePhoto
    {
        [Key]
        public Guid? Id { get; set; }

        public string Url { get; set; }
    }
}
