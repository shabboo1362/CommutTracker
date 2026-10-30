using CommuteTracker.Api.DTOs;
using CommuteTracker.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommuteTracker.Core.Entities;

namespace CommuteTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly ITripService _tripService;
    public TripController( ITripService tripService)
    {
        _tripService = tripService;
    }
    [Authorize]
    [HttpPost]
   
    public async Task<IActionResult> CreateTrip(CreateTripRequest request)
    {
        try
    {
        var points = request.Points.Select(p => new LocationPoint
        {
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            Timestamp = p.Timestamp
        }).ToList();

        var result = await _tripService.CreateTripAsync(
            request.UserId,
            request.TransportType,
            points);

        return Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
    }
    [Authorize]
    [HttpGet("user/{userId}")]
public async Task<IActionResult> GetUserTrips(Guid userId)
{
    var result = await _tripService.GetUserTripsAsync(userId);

    return Ok(result);
}
[Authorize]
[HttpPost("{tripId}/end")]
public async Task<IActionResult> EndTrip(Guid tripId)
{
    var result = await _tripService.EndTripAsync(tripId);

    if (result == null)
    {
        return NotFound("Trip not found.");
    }

    return Ok(result);
}
}