using CW_7_s2991.Models;
using CW_7_s2991.Models.DTOs;

namespace CW_7_s2991.Services;

public class ClientsServicePrimary(IConfiguration config) : IClientsService
{
    public Task<IEnumerable<TripGetDTO>> GetTripsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Client> PostCreateClientAsync(ClientCreateDTO client)
    {
        throw new NotImplementedException();
    }

    public Task<ClientTrip> PutRegisterClientsTripAsync(int idClient, int idTrip)
    {
        throw new NotImplementedException();
    }

    public Task<ClientTrip> DeleteClientsTripAsync(int idClient, int idTrip)
    {
        throw new NotImplementedException();
    }
}