using CdrPlatform.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CdrDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Ensure it exists somewhere, not ideal as migrations apparently
using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CdrDbContext>();
    db.Database.EnsureCreated();
}

app.Run();