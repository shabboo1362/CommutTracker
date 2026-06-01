namespace CommuteTracker.Core.Services;

public static class SpeedCalculator
{
    public static double CalculateAverageSpeed(
        double distanceKm,
        DateTime start,
        DateTime end)
    {
        var durationHours = (end - start).TotalHours;

        if (durationHours <= 0)
            return 0;

        return distanceKm / durationHours;
    }
}