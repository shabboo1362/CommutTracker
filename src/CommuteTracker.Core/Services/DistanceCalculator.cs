using System;
using System.Collections.Generic;
using System.Linq;

namespace CommuteTracker.Core.Services
{
    public static class DistanceCalculator
    {
        /// <summary>
        /// Calculates the great-circle distance between two geographic points using the Haversine formula.
        /// </summary>
        /// <param name="lat1">Latitude of first point in degrees</param>
        /// <param name="lon1">Longitude of first point in degrees</param>
        /// <param name="lat2">Latitude of second point in degrees</param>
        /// <param name="lon2">Longitude of second point in degrees</param>
        /// <returns>Distance in kilometers</returns>
        const double EarthRadiusKm = 6371.0;
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // calculate Total distance of a trip by summing distances between consecutive location points
        public static double CalculateTotalDistance(List<Core.Entities.LocationPoint> Points)
        {
            if (Points == null || Points.Count < 2)
                return 0.0;
            double totalDistance = 0.0;
            var orderedPoints = Points.OrderBy(p => p.Timestamp).ToList();
            for (int i = 0; i < orderedPoints.Count - 1; i++)
            {
                var point1 = orderedPoints[i];
                var point2 = orderedPoints[i + 1];
                totalDistance += CalculateDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);
            }
            return Math.Round(totalDistance, 2); // round to 2 decimal places for better readability
        }
    }
}