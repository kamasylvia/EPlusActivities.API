using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class GeneralLotteryRecords
    {
        [Required]
        public Guid? Id { get; set; }

        public virtual Activity Activity { get; set; }

        public ChannelCode Channel { get; set; }

        public DateTime DateTime { get; set; }

        public int Draws { get; set; }

        public int Winners { get; set; }

        public int Redemption { get; set; }
    }
}
