using CdrPlatform.Database;
using CdrPlatform.Endpoints;
using CdrPlatform.Extensions;
using CdrPlatform.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSerilog();
});

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<CdrDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCdrPlatformServices();

var app = builder.Build();

app.MapGet("/", () => "Welcome to the awesome CDR Platform!");
app.MapPost("/data/load", async (string filePath, CsvDataLoader loader) =>
{
    if (string.IsNullOrEmpty(filePath))
    {
        return Results.BadRequest("File path is required.");
    }
    if (!File.Exists(filePath))
    {
        return Results.NotFound("File not found.");
    }
    
    try
    {
        await loader.LoadCsvToDbAsync(filePath);
        return Results.Ok("File loaded successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
app.MapGroup("/api")
    .MapRecordsApi()
    .MapCostsApi()
    .MapCurrenciesApi();

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