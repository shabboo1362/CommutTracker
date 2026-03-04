//test distance calculator
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Services;
namespace CommuteTracker.Tests
{
    public class DistanceCalculatorTests
    {
        [Fact]
        public void CalculateDistance_NoPoint_ReturnsZero()
        {
            var points = new List<LocationPoint>();
            var result = DistanceCalculator.CalculateTotalDistance(points);

            Assert.Equal(0, result);
        }
    
        [Fact]
        public void CalculateDistance_SamePoint_ReturnsZero()
        {
            var points = new List<LocationPoint>
             { new LocationPoint { Latitude = 35.6892, Longitude = 51.3890, Timestamp = DateTime.UtcNow } };
            

            var result = DistanceCalculator.CalculateTotalDistance(points);

            Assert.Equal(0, result);
        }
    

        [Fact]
        public void CalculateTotalDistance_TwoPoints_ReturnsSumOfSegments()
        {
            var now = DateTime.UtcNow;
            var points = new List<LocationPoint>
            {
                new LocationPoint{ Latitude = 35.6892, Longitude = 51.3890, Timestamp = now },
                new LocationPoint{ Latitude = 35.7000, Longitude = 51.4000, Timestamp = now.AddMinutes(5) },
    
            };

            var total = DistanceCalculator.CalculateTotalDistance(points);

            Assert.True(total > 0);
        }
    }
}