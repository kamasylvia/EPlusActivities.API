using System;
using System.ComponentModel.DataAnnotations;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.DTOs.LotteryDtos
{
    public class LotteryForUpdateDto
    {
        [Required]
        public Guid? Id { get; set; }

        public Channel Channel { get; set; }

        public bool IsLucky { get; set; }

        public int UsedCredit { get; set; }

        public Guid? UserId { get; set; }

        public Guid? ActivityId { get; set; }

        public Guid? PrizeItemId { get; set; }

        public Guid? PrizeTypeId { get; set; }
    }
}
