using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeTypePrizeItem
    {
        [Key]
        public Guid? Id { get; set; }

        public PrizeItem PrizeItem { get; set; }

        public PrizeType PrizeType { get; set; }
    }
}
