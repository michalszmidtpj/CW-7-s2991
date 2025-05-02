using CW_7_s2991.Models.DTOs;

namespace CW_7_s2991.Services;

public interface ITripsService
{
    public Task<IEnumerable<TripGetDTO>> GetTripsAsync();
}