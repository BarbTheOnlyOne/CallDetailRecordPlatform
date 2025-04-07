using CdrPlatform.Abstractions;
using CdrPlatform.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CdrPlatform.Endpoints;

/// <summary>
///     Class aggregating all the endpoints related to the currencies.
///     Motivation behind this was to keep the logically related types of endpoints together, making it more clean and
///     separated.
/// </summary>
public static class CurrenciesEndpoints
{
    public static RouteGroupBuilder MapCurrenciesApi(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/currencies", GetCurrenciesAsync);
        groupBuilder.MapGet("/currencies/{callerId}", GetCurrenciesByCallerAsync);

        return groupBuilder;
    }

    public static async Task<Results<Ok<List<Currency>>, ProblemHttpResult, NotFound>> GetCurrenciesAsync(
        CdrDbContext context, ILogger logger)
    {
        List<Currency> currencies;
        try
        {
            currencies = await context.CallDetailRecords
                .Select(r => r.Currency)
                .Distinct()
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return currencies.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(currencies);
    }

    public static async Task<Results<Ok<List<Currency>>, ProblemHttpResult, NotFound>> GetCurrenciesByCallerAsync(
        int callerId, CdrDbContext context, ILogger logger)
    {
        List<Currency> currencies;
        try
        {
            currencies = await context.CallDetailRecords
                .Where(r => r.CallerId == callerId)
                .Select(r => r.Currency)
                .Distinct()
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return currencies.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(currencies);
    }
}