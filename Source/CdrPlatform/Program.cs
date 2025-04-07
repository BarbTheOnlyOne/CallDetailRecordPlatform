using CdrPlatform;
using CdrPlatform.Abstractions;
using CdrPlatform.Database;
using CdrPlatform.Endpoints;
using CdrPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<CdrDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Welcome to the awesome CDR Platform!");
app.MapGroup("/api")
    .MapRecordsApi()
    .MapCostsApi();

app.MapGet("/api/currencies", (CdrDbContext context) =>
{
    List<Currency> currencies;
    try
    {
        currencies = context.CallDetailRecords
            .Select(r => r.Currency)
            .Distinct()
            .ToList();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return currencies.Count == 0 ? Results.NotFound() : Results.Ok(currencies);
});
app.MapGet("/api/currencies/{callerId}", (int callerId, CdrDbContext context) =>
{
    List<Currency> currencies;
    try
    {
        currencies = context.CallDetailRecords
            .Where(r => r.CallerId == callerId)
            .Select(r => r.Currency)
            .Distinct()
            .ToList();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return currencies.Count == 0 ? Results.NotFound() : Results.Ok(currencies);
});

// Ensure it exists somewhere, not ideal as migrations apparently
using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CdrDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Run();