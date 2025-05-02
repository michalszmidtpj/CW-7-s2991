using CW_7_s2991.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddTransient<IClientsService, ClientsServicePrimary>();
builder.Services.AddTransient<ITripsService, TripsServicePrimary>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();