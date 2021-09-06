using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Dtos.LotteryDtos
{
    public class LotteryDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string ChannelCode { get; set; }

        public string LotteryDisplay { get; set; }

        public bool IsLucky { get; set; }

        public bool PickedUp { get; set; }

        public bool Delivered { get; set; }

        public DateTime? PickedTime { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        public Guid? ActivityId { get; set; }

        public Guid? PrizeItemId { get; set; }

        public Guid? PrizeTierId { get; set; }
    }
}
