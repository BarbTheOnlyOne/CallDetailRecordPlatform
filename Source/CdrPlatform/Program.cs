using CdrPlatform.Abstractions;
using CdrPlatform.Database;
using CdrPlatform.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CdrDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    
    context.CallDetailRecords.Add(testRecord);
    
    await context.SaveChangesAsync();
    
    return Results.Ok("Inserted test record");
});

app.MapGet("/dbtest/get", async (CdrDbContext context) =>
{
    var records = await context.CallDetailRecords.ToListAsync();
    return Results.Ok(records);
});

// Ensure it exists somewhere, not ideal as migrations apparently
using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CdrDbContext>();
    db.Database.EnsureCreated();
}

app.Run();