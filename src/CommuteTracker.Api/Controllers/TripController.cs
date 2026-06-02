using CommuteTracker.Api.DTOs;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Services;
using CommuteTracker.Infrastructure;
using CommuteTracker.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommuteTracker.Core.Services;

namespace CommuteTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly CommuteTrackerDbContext _db;
    private readonly TripService _tripService;
    public TripController(CommuteTrackerDbContext db, TripService tripService)
    {
        _db = db;
        _tripService = tripService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrip(CreateTripRequest request)
    {
        var user = await _db.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound("User not found");

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            StartTime = request.Points.First().Timestamp,
            EndTime = request.Points.Last().Timestamp,
            TransportType = request.TransportType,
            Status = TripStatus.Active
        };

        var points = request.Points.Select(p => new LocationPoint
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            Timestamp = p.Timestamp
        }).ToList();

        trip.LocationPoints = points;

        
        var totalDistance = _tripService.CalculateDistance(points);
        
        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            trip.Id,
            Distance = totalDistance
        });
    }
    [HttpGet("user/{userId}")]
public async Task<IActionResult> GetUserTrips(Guid userId)
{
    var trips = _db.Trips
    .Include(t => t.LocationPoints)
    .Where(t => t.UserId == userId)
    .ToList();
    foreach (var trip in trips)
    {
        _tripService.AutoCompleteTrip(trip);
    }
    var result = trips.Select(t =>
{
    var distance = _tripService.CalculateDistance(t.LocationPoints);

    var speed = t.EndTime.HasValue
        ? _tripService.CalculateSpeed(
            distance,
            t.StartTime,
            t.EndTime.Value)
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
});
    return Ok(result);
}
[HttpPost("{tripId}/end")]
public async Task<IActionResult> EndTrip(Guid tripId)
{
    var trip = await _db.Trips
        .Include(t => t.LocationPoints)
        .FirstOrDefaultAsync(t => t.Id == tripId);

    if (trip == null)
        return NotFound();

    trip.EndTime = DateTime.UtcNow;
    trip.Status = TripStatus.Completed;

    var distance = _tripService.CalculateDistance(trip.LocationPoints);

    await _db.SaveChangesAsync();

    return Ok(new
    {
        trip.Id,
        Status = trip.Status,
        Distance = Math.Round(distance, 2)
    });
}
}