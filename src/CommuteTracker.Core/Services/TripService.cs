using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Services;
using CommuteTracker.Core.Helpers;
using CommuteTracker.Core.Enums;

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
    public bool IsTripInactive(DateTime lastPointTime, int timeoutMinutes = 5)
    {
        return (DateTime.UtcNow - lastPointTime).TotalMinutes > timeoutMinutes;
    }

    public void AutoCompleteTrip(Trip trip)
    {
        if (trip.Status == TripStatus.Completed)
            return;

        var lastPoint = trip.LocationPoints
            .OrderByDescending(p => p.Timestamp)
            .FirstOrDefault();

        if (lastPoint == null)
            return;

        if (IsTripInactive(lastPoint.Timestamp))
        {
            trip.Status = TripStatus.Completed;
            trip.EndTime = lastPoint.Timestamp;
        }
    }
}