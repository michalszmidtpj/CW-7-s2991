using CW_7_s2991.Models;
using CW_7_s2991.Models.DTOs;

namespace CW_7_s2991.Services;

public interface IClientsService
{
    public Task<IEnumerable<ClientTripGetDTO>> GetTripsAsync(int id);
    public Task<Client> PostCreateClientAsync(ClientCreateDTO client);
    public Task<ClientTrip> PutRegisterClientsTripAsync(int idClient, int idTrip);
    public Task<ClientTrip> DeleteClientsTripAsync(int idClient, int idTrip);
    
}