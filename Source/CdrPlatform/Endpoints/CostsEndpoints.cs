using CdrPlatform.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CdrPlatform.Endpoints;

/// <summary>
///     Class aggregating all the endpoints related to the costs.
///     Motivation behind this was to keep the logically related types of endpoints together, making it more clean and
///     separated.
/// </summary>
public static class CostsEndpoints
{
    public static RouteGroupBuilder MapCostsApi(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/costs/{callerId}/{year}/{month}", GetCostsByCallerAndMonthAsync);
        groupBuilder.MapGet("/costs/{callerId}/{year}", GetCostsByCallerAndYearAsync);

        return groupBuilder;
    }

    public static async Task<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>> GetCostsByCallerAndMonthAsync(
        int callerId, int year, int month, CdrDbContext context, ILogger logger)
    {
        List<decimal> costs;
        try
        {
            costs = await context.CallDetailRecords
                .Where(r => r.CallerId == callerId && r.CallDate.Year == year && r.CallDate.Month == month)
                .Select(r => r.Cost)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return costs.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(costs);
    }

    public static async Task<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>> GetCostsByCallerAndYearAsync(
        int callerId, int year, int month, CdrDbContext context, ILogger logger)
    {
        List<decimal> costs;
        try
        {
            costs = await context.CallDetailRecords
                .Where(r => r.CallerId == callerId && r.CallDate.Year == year)
                .Select(r => r.Cost)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return costs.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(costs);
    }
}