using System.Text.RegularExpressions;
using CW_7_s2991.Exceptions;
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
        
        // wzbogadzenie danych joinem z tabeli i utworzenie z niego customowego dto
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

    public async Task<int> PostCreateClientAsync(ClientCreateDTO client)
    {
        var errors = new List<string>();

        if (!IsValidEmail(client.Email))
            errors.Add("Email");
        if (!IsValidPesel(client.Pesel))
            errors.Add("PESEL");
        if (!IsValidInternationalPhone(client.Telephone))
            errors.Add("Telephone");

        if (errors.Count > 0)
            throw new IllegalParamException(
                "Invalid parameter(s): " + string.Join(", ", errors)
            );

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // wstawienie klienta do db
        const string insertSql = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) 
        VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
    ";
        await using (var insertCmd = new SqlCommand(insertSql, connection))
        {
            insertCmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            insertCmd.Parameters.AddWithValue("@LastName", client.LastName);
            insertCmd.Parameters.AddWithValue("@Email", client.Email);
            insertCmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            insertCmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await insertCmd.ExecuteNonQueryAsync();
        }

        // po wstawieniu, odnajdujemy id nadane przez db
        const string selectSql = @"
        SELECT IdClient 
          FROM Client 
         WHERE Pesel = @Pesel;
    ";
        await using (var selectCmd = new SqlCommand(selectSql, connection))
        {
            selectCmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            object result = await selectCmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Unable to retrieve new Client ID.");

            return Convert.ToInt32(result);
        }
    }

    public async Task PutRegisterClientsTripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // sprawdzenie czy klient istnieje
        const string clientCheckSql = @"SELECT COUNT(1) FROM Client WHERE IdClient = @IdClient";
        await using (var clientCmd = new SqlCommand(clientCheckSql, connection))
        {
            clientCmd.Parameters.AddWithValue("@IdClient", idClient);
            var clientExists = (int)await clientCmd.ExecuteScalarAsync() > 0;
            if (!clientExists)
                throw new KeyNotFoundException($"Client with ID {idClient} not found.");
        }


        int maxPeople;
        // sprawdzenie czy wycieczka istnieje i max l. osob
        const string tripCheckSql = @"SELECT MaxPeople FROM Trip WHERE IdTrip = @IdTrip";
        await using (var tripCmd = new SqlCommand(tripCheckSql, connection))
        {
            tripCmd.Parameters.AddWithValue("@IdTrip", idTrip);
            var result = await tripCmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                throw new KeyNotFoundException($"Trip with ID {idTrip} not found.");
            maxPeople = Convert.ToInt32(result);
        }

        // sprawdzenie ile osob juz jest zarejestrownaych na wycieczke
        const string countSql = @"SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @IdTrip";
        await using (var countCmd = new SqlCommand(countSql, connection))
        {
            countCmd.Parameters.AddWithValue("@IdTrip", idTrip);
            var registeredCount = (int)await countCmd.ExecuteScalarAsync();
            if (registeredCount >= maxPeople)
                throw new InvalidOperationException("The trip has reached its maximum number of participants.");
        }

    // sprawdzenie czy kleint nie jest juz zarejestrowany na wycieczke 
        const string duplicateCheckSql =
            @"SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @IdTrip AND IdClient = @IdClient";
        await using (var dupCmd = new SqlCommand(duplicateCheckSql, connection))
        {
            dupCmd.Parameters.AddWithValue("@IdTrip", idTrip);
            dupCmd.Parameters.AddWithValue("@IdClient", idClient);
            var already = (int)await dupCmd.ExecuteScalarAsync() > 0;
            if (already)
                throw new InvalidOperationException("Client is already registered for this trip.");
        }
        
        long unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();  // :contentReference[oaicite:0]{index=0}

        // wstawienie rekordu po sprawdzeniach
        const string insertSql = @"
        INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
        VALUES (@IdClient, @IdTrip, @RegisteredAt);
    ";

        await using var insertCmd = new SqlCommand(insertSql, connection);
        insertCmd.Parameters.AddWithValue("@IdClient",     idClient);
        insertCmd.Parameters.AddWithValue("@IdTrip",       idTrip);
        insertCmd.Parameters.AddWithValue("@RegisteredAt", (int)unixSeconds);
        await insertCmd.ExecuteNonQueryAsync();
    }


    public async Task DeleteClientsTripAsync(int idClient, int idTrip)
    {
        int isthere = 0;
        int deletedcount = 0;

        await using var connection = new SqlConnection(_connectionString);
        // sprawdzenie czy istnieje taki rekord
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
            // usuniecie tego rekordu
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


    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        const string pattern =
            @"^(?("")("".+?""@)|(([0-9a-zA-Z]" +
            @"((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" +
            @"(?<=[0-9a-zA-Z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|" +
            @"(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+" +
            @"[a-zA-Z]{2,}))$";

        return Regex.IsMatch(email, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public static bool IsValidInternationalPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        const string pattern =
            @"^((\+|00)\d{1,3}[ -]?)?(\d[ -]?){6,14}\d$";

        return Regex.IsMatch(phone, pattern, RegexOptions.Compiled);
    }

    public static bool IsValidPesel(string pesel)
    {
        if (string.IsNullOrWhiteSpace(pesel))
            return false;

        const string pattern =
            @"^\d{2}((0[1-9])|(1[0-2]))" +
            @"((0[1-9])|([12][0-9])|(3[01]))" +
            @"\d{5}$";

        return Regex.IsMatch(pesel, pattern, RegexOptions.Compiled);
    }
}