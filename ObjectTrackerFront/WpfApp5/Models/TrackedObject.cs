using System;

namespace ObjectTrackerFront.Models
{
    public class TrackedObject
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
