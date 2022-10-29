using FuelQueManagement_Service.Models;
using FuelQueManagement_Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add the  services to the container
builder.Services.Configure<DatabaseConnection>(
    builder.Configuration.GetSection("DatabaseConnection"));

// build the services
builder.Services.AddSingleton<FuelStationService>();
builder.Services.AddSingleton<FuelService>();
builder.Services.AddSingleton<QueueService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuring  the HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
