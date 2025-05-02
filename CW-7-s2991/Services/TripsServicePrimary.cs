using CW_7_s2991.Models.DTOs;

namespace CW_7_s2991.Services;

public class TripsServicePrimary(IConfiguration config) : ITripsService
{
    public Task<IEnumerable<TripGetDTO>> GetTripsAsync()
    {
        throw new NotImplementedException();
    }
}