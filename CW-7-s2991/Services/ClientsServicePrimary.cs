using CW_7_s2991.Models;
using CW_7_s2991.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace CW_7_s2991.Services;

public class ClientsServicePrimary(IConfiguration config) : IClientsService
{
    private readonly string? _connectionString = config.GetConnectionString("Default");

    public async Task<IEnumerable<ClientTripGetDTO>> GetTripsAsync(int id)
    {
        var result = new List<ClientTripGetDTO>();
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           select ct.IdTrip, ct.RegisteredAt, ct.PaymentDate, t.name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople from Client_Trip ct left join dbo.Trip T on ct.IdTrip = T.IdTrip
                           where ct.IdClient = @id;
                           """;
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())

        {
            result.Add(new ClientTripGetDTO
            {
                IdTrip = reader.GetInt32(0),
                RegisteredAt = reader.GetInt32(1),
                PaymentDay = reader.GetInt32(2),
                Name = reader.GetString(3),
                Description = reader.GetString(4),
                DateFrom = reader.GetDateTime(5),
                DateTo = reader.GetDateTime(6),
                MaxPeople = reader.GetInt32(7),
            });

            
        }
        return result;
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