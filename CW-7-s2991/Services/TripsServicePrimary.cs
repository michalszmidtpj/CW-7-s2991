
using CW_7_s2991.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace CW_7_s2991.Services;

public class TripsServicePrimary(IConfiguration config) : ITripsService
{
    private readonly string? _connectionString = config.GetConnectionString("Default");
    public async Task<IEnumerable<TripGetDTO>> GetTripsAsync()
    {
        var result = new List<TripGetDTO>();
        await using var connection = new SqlConnection(_connectionString);
        // zwróci dto wzbogacony o informacje z 3 tabel zgodnie z poleceniem.
        const string sql = """
                           select t.idtrip, t.name, t.Description, t.DateFrom, t.dateto, t.MaxPeople, c.Name as CountryName
                           from trip t
                                    left join Country_Trip ct on t.IdTrip = ct.IdTrip
                                    inner join Country C on ct.IdCountry = C.IdCountry
                           """;
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            
        {
            result.Add(new TripGetDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                CountryName = reader.GetString(6),
            });
        }

        return result;
    }
}