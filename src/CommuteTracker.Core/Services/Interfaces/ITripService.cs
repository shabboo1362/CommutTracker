using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommuteTracker.Core.Entities;

namespace CommuteTracker.Core.Services.Interfaces
{
    public interface ITripService
    {
         Task<object> CreateTripAsync(
        Guid userId,
        TransportType transportType,
        List<LocationPoint> points);

    Task<object?> GetUserTripsAsync(Guid userId);

    Task<object?> EndTripAsync(Guid tripId);
    }
}
