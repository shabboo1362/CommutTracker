using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommuteTracker.Core.Services.Interfaces;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Enums;
using CommuteTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CoreTripService = CommuteTracker.Core.Services.TripService;

namespace CommuteTracker.Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly CommuteTrackerDbContext _db;
        private readonly CoreTripService _coreTripService;

    public TripService(CommuteTrackerDbContext db)
    {
        _db = db;
        _coreTripService = new CoreTripService();
    }
    public async Task<object> CreateTripAsync(
    Guid userId,
    TransportType transportType,
    List<LocationPoint> points)
{
    var user = await _db.Users.FindAsync(userId);

    if (user == null)
        throw new KeyNotFoundException("User not found");

    if (points == null || points.Count < 2)
        throw new ArgumentException("At least two location points are required.");

    var orderedPoints = points
        .OrderBy(p => p.Timestamp)
        .ToList();

    var trip = new Trip
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        StartTime = orderedPoints.First().Timestamp,
        EndTime = orderedPoints.Last().Timestamp,
        TransportType = transportType,
        Status = TripStatus.Active
    };

    var locationPoints = orderedPoints.Select(p => new LocationPoint
    {
        Id = Guid.NewGuid(),
        TripId = trip.Id,
        Latitude = p.Latitude,
        Longitude = p.Longitude,
        Timestamp = p.Timestamp
    }).ToList();

    trip.LocationPoints = locationPoints;

    var distance = _coreTripService.CalculateDistance(locationPoints);

    _db.Trips.Add(trip);
    await _db.SaveChangesAsync();

    return new
    {
        trip.Id,
        Distance = Math.Round(distance, 2)
    };
}
public async Task<object?> GetUserTripsAsync(Guid userId)
{
    var trips = await _db.Trips
        .Include(t => t.LocationPoints)
        .Where(t => t.UserId == userId)
        .ToListAsync();

    foreach (var trip in trips)
    {
        _coreTripService.AutoCompleteTrip(trip);
    }

    await _db.SaveChangesAsync();

    var result = trips.Select(t =>
    {
        var distance = _coreTripService.CalculateDistance(t.LocationPoints);

        var speed = t.EndTime.HasValue
            ? _coreTripService.CalculateSpeed(
                distance,
                t.StartTime,
                t.EndTime)
            : 0;

        return new
        {
            t.Id,
            t.StartTime,
            t.EndTime,
            t.TransportType,
            Status = t.Status,
            IsActive = t.Status == TripStatus.Active,
            Distance = Math.Round(distance, 2),
            AverageSpeedKmH = Math.Round(speed, 2),

            Points = t.LocationPoints.Select(p => new
            {
                p.Latitude,
                p.Longitude,
                p.Timestamp
            })
        };
    }).ToList();

    return result;
}
public async Task<object?> EndTripAsync(Guid tripId)
{
    var trip = await _db.Trips
        .Include(t => t.LocationPoints)
        .FirstOrDefaultAsync(t => t.Id == tripId);

    if (trip == null)
        return null;

    if (trip.Status == TripStatus.Completed)
        throw new InvalidOperationException("Trip is already completed.");

    trip.EndTime = DateTime.UtcNow;
    trip.Status = TripStatus.Completed;

    var distance = _coreTripService.CalculateDistance(trip.LocationPoints);

    await _db.SaveChangesAsync();

    return new
    {
        trip.Id,
        Status = trip.Status,
        Distance = Math.Round(distance, 2)
    };
}
    }
}
