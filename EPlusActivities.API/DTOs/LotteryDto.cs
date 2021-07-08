using System;

namespace EPlusActivities.API.DTOs
{
    public class LotteryDto
    {
        public Guid WinnerId { get; set; }
        public string Channel { get; set; }
        public string PhoneNumber { get; set; }
    }
}