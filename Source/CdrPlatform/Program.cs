using CdrPlatform.Abstractions;
using CdrPlatform.Database;
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

app.MapGet("/", () => "Welcome to the awesome CDR Platfom!");
app.MapGet("/api/records", async (CdrDbContext context) =>
{
    List<CallDetailRecord> records;
    try
    {
        records = await context.CallDetailRecords.ToListAsync();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching records: {error}", e.Message);
        return Results.Problem("Error while fetching records", 
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return Results.Ok(records);
});
app.MapGet("/api/records/{callerId}", async (int callerId, CdrDbContext context) =>
{
    List<CallDetailRecord> records;
    try
    {
        records = await context.CallDetailRecords.Where(r => r.CallerId == callerId).ToListAsync();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return records.Count == 0 ? Results.NotFound() : Results.Ok(records);
});
app.MapGet("/api/records/{callerId}/{year}/{month}", (int callerId, int year, int month, CdrDbContext context) =>
{
    List<CallDetailRecord> records;
    try
    {
        records = context.CallDetailRecords
            .Where(r => r.CallerId == callerId && r.CallDate.Year == year && r.CallDate.Month == month)
            .ToList();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }
    
    return records.Count == 0 ? Results.NotFound() : Results.Ok(records);
});
app.MapGet("/api/costs/{callerId}/{year}/{month}", (int callerId, int year, int month, CdrDbContext context) =>
{
    List<decimal> costs;
    try
    {
        costs = context.CallDetailRecords
            .Where(r => r.CallerId == callerId && r.CallDate.Year == year && r.CallDate.Month == month)
            .Select(r => r.Cost)
            .ToList();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }
    
    return costs.Count == 0 ? Results.NotFound() : Results.Ok(costs);
});
app.MapGet("/api/costs/{callerId}/{year}", (int callerId, int year, CdrDbContext context) =>
{
    List<decimal> costs;
    try
    {
        costs = context.CallDetailRecords
            .Where(r => r.CallerId == callerId && r.CallDate.Year == year)
            .Select(r => r.Cost)
            .ToList();
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }
    
    return costs.Count == 0 ? Results.NotFound() : Results.Ok(costs);
});
app.MapGet("/api/record/{recordReference}", (string recordReference, CdrDbContext context) =>
{
    CallDetailRecord? record;
    try
    {
        record = context.CallDetailRecords
            .FirstOrDefault(r => r.Reference == recordReference);
    }
    catch (Exception e)
    {
        app.Logger.LogError("Error while fetching record: {error}", e.Message);
        return Results.Problem("Error while fetching record", 
            statusCode: StatusCodes.Status500InternalServerError);
    }
    
    return record == null ? Results.NotFound() : Results.Ok(record);
});
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