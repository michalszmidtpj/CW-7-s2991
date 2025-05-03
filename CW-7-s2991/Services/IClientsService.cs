using CW_7_s2991.Models;
using CW_7_s2991.Models.DTOs;

namespace CW_7_s2991.Services;

public interface IClientsService
{
    public Task<IEnumerable<ClientTripGetDTO>> GetTripsAsync(int id);
    public Task<int> PostCreateClientAsync(ClientCreateDTO client);
    public Task PutRegisterClientsTripAsync(int idClient, int idTrip);
    public Task DeleteClientsTripAsync(int idClient, int idTrip);
    
}