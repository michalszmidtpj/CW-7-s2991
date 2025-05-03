using CW_7_s2991.Exceptions;
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

    public async Task<Client> PostCreateClientAsync(ClientCreateDTO client)
    {
        throw new NotImplementedException();
    }

    public async Task<ClientTrip> PutRegisterClientsTripAsync(int idClient, int idTrip)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteClientsTripAsync(int idClient, int idTrip)
    {

        int isthere = 0;
        int deletedcount = 0;

        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                           select 1 from Client_Trip 
                                    where IdClient = @idClient 
                                    and IdTrip = @idTrip;
                           """;
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idClient", idClient);
        command.Parameters.AddWithValue("@idTrip", idTrip);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
            isthere += reader.GetInt32(0);

        reader.Close();
        
        if (isthere > 0)
        {
            const string sql2 = """
                               DELETE FROM Client_Trip
                               WHERE IdClient = @idClient
                                 AND IdTrip   = @idTrip;
                               """;
            await using var delcommand = new SqlCommand(sql2, connection);
            delcommand.Parameters.AddWithValue("@idClient", idClient);
            delcommand.Parameters.AddWithValue("@idTrip", idTrip);

            deletedcount = await delcommand.ExecuteNonQueryAsync();
        }

        if (deletedcount == 0 || isthere == 0)
            throw new NoSuchClientTripException("Provided client did not register such trip");
        
    }
}