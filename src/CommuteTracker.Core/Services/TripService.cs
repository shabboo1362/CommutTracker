using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Services;
using CommuteTracker.Core.Helpers;

namespace CommuteTracker.Core.Services;

public class TripService
{
    public double CalculateDistance(List<LocationPoint> points)
    {
        return DistanceCalculator.CalculateTotalDistance(points);
    }

    public double CalculateSpeed(double distance, DateTime start, DateTime? end)
    {
        if (!end.HasValue)
            return 0;

        return SpeedCalculator.CalculateAverageSpeed(
            distance,
            start,
            end.Value);
    }
}