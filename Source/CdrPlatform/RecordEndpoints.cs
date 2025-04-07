using CdrPlatform.Database;
using CdrPlatform.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CdrPlatform;

public static class RecordEndpoints
{
    public static RouteGroupBuilder MapRecordsApi(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/record/{recordReference}", GetRecordByReferenceAsync);
        
        groupBuilder.MapGet("/records", GetRecordsAsync);
        groupBuilder.MapGet("/records/{callerId}", GetRecordsByCallerAsync);
        groupBuilder.MapGet("/records/{callerId}/{year}/{month}", GetRecordsByCallerAndMonthAsync);
        
        return groupBuilder;
    }
    
    public static Results<Ok<CallDetailRecord>, ProblemHttpResult, NotFound> GetRecordByReferenceAsync(string reference, CdrDbContext context, ILogger logger)
    {
        CallDetailRecord? record;
        try
        {
            record = context.CallDetailRecords
                .FirstOrDefault(r => r.Reference == reference);
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return record == null ? TypedResults.NotFound() : TypedResults.Ok(record);
    }
    
    public static async Task<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>> GetRecordsAsync(CdrDbContext context, ILogger logger)
    {
        List<CallDetailRecord> records;
        try
        {
            records = await context.CallDetailRecords.ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return records.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(records);
    }
    
    public static async Task<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>> GetRecordsByCallerAsync(int callerId, CdrDbContext context, ILogger logger)
    {
        List<CallDetailRecord> records;
        try
        {
            records = await context.CallDetailRecords.Where(r => r.CallerId == callerId).ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return records.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(records);
    }
    
    public static async Task<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>> GetRecordsByCallerAndMonthAsync(int callerId, int year, int month, CdrDbContext context, ILogger logger)
    {
        List<CallDetailRecord> records;
        try
        {
            records = await context.CallDetailRecords
                .Where(r => r.CallerId == callerId && r.CallDate.Year == year && r.CallDate.Month == month)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Error while fetching records: {error}", e.Message);
            return TypedResults.Problem("Error while fetching records");
        }

        return records.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(records);
    }
}