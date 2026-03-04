namespace CommuteTracker.Core.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 
        public DateTime StartTime { get; set; } 
        public DateTime? EndTime { get; set; }
        public TransportType TransportType { get; set; }
        public User? User { get; set; }
        public List<LocationPoint> LocationPoints { get; set; } = new ();
    }
}