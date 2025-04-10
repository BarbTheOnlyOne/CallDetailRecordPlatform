using CdrPlatform.Database;
using CdrPlatform.Services;

namespace CdrPlatform.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCdrPlatformServices(this IServiceCollection services)
    {
        services.AddScoped<CsvDataLoader>();
        services.AddScoped<CdrDbContext>();

        return services;
    }
}