namespace CommuteTracker.Core.Entities
{
    public class LocationPoint
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public Trip? Trip { get; set; }
    }
}