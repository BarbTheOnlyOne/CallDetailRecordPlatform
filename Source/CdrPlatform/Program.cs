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

app.MapGet("/", () => "Hello World!");

app.MapGet("/dbtest/insert", async (CdrDbContext context) =>
{
    var testRecord = new CallDetailRecord
    {
        CallerId = 1234567890,
        Recipient = 987456321,
        CallDate = DateOnly.FromDateTime(DateTime.Now),
        EndTime = TimeSpan.FromMinutes(12),
        Duration = 42,
        Cost = 20,
        Reference = "TestReference",
        Currency = Currency.GBP
    };
    
    app.Logger.LogInformation("Inserting record {@testRecord}", testRecord);
    
    context.CallDetailRecords.Add(testRecord);
    
    await context.SaveChangesAsync();
    
    app.Logger.LogInformation($"Inserted record & saved to DB.");
    
    return Results.Ok("Inserted test record");
});

app.MapGet("/dbtest/get", async (CdrDbContext context) =>
{
    var records = await context.CallDetailRecords.ToListAsync();
    
    app.Logger.LogInformation("Fetched {numberOfRecords} records from DB.", records.Count);
    
    return Results.Ok(records);
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