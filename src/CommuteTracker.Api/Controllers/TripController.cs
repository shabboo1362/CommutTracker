using CommuteTracker.Api.DTOs;
using CommuteTracker.Core.Entities;
using CommuteTracker.Core.Services;
using CommuteTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CommuteTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly CommuteTrackerDbContext _db;

    public TripController(CommuteTrackerDbContext db)
    {
        _db = db;
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
            TransportType = request.TransportType
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

        
        var totalDistance = DistanceCalculator.CalculateTotalDistance(points);

        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            trip.Id,
            Distance = totalDistance
        });
    }
}