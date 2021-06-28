using System;

namespace EPlusActivities.Entities
{
    public class Activity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string WinningResultId { get; set; }
        public WinningResult WinningResult { get; set; }
    }
}