using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Entities
{
    public class LotterySummary
    {
        [Required]
        public Guid? Id { get; set; }

        public Guid? ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public ChannelCode Channel { get; set; }

        public DateOnly Date { get; set; }

        public int Draws { get; set; }

        public int Winners { get; set; }

        public int Redemption { get; set; }
    }
}
