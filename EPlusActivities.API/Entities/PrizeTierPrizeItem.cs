using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeTierPrizeItem
    {
        public Guid? PrizeItemId { get; set; }

        public virtual PrizeItem PrizeItem { get; set; }

        public Guid? PrizeTierId { get; set; }

        public virtual PrizeTier PrizeTier { get; set; }
    }
}
