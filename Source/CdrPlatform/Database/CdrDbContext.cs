using CdrPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace CdrPlatform.Database;

public class CdrDbContext(DbContextOptions<CdrDbContext> options) : DbContext(options)
{
    public DbSet<CallDetailRecord> CallDetailRecords { get; set; }
}