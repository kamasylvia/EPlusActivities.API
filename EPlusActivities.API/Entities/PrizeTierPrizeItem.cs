using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.Entities
{
    public class PrizeTierPrizeItem
    {
        public Guid? PrizeItemId { get; set; }

        public PrizeItem PrizeItem { get; set; }

        public Guid? PrizeTierId { get; set; }

        public PrizeTier PrizeTier { get; set; }
    }
}
