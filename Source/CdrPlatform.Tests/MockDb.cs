using CdrPlatform.Database;
using Microsoft.EntityFrameworkCore;

namespace CdrPlatform.Tests;

public class MockDb : IDbContextFactory<CdrDbContext>
{
    public CdrDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CdrDbContext>()
            .UseInMemoryDatabase($"{Guid.NewGuid().ToString()}")
            .Options;

        return new CdrDbContext(options);
    }
}