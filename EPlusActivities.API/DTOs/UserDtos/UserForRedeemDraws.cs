using System;
using System.ComponentModel.DataAnnotations;

namespace EPlusActivities.API.DTOs.UserDtos
{
    public class UserForRedeemDraws
    {
        [Required]
        public Guid? Id { get; set; }

        public int Count { get; set; }

        public int UnitPrice { get; set; }

        public string Reason { get; set; }
    }
}
