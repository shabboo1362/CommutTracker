using CommuteTracker.Core.Entities;

namespace CommuteTracker.Api.DTOs
{
    public class CreateTripRequest
    {
        public Guid UserId { get; set; }
        public TransportType TransportType { get; set; }
        public List<LocationPointDto> Points { get; set; }= new();
    }

    public class LocationPointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}