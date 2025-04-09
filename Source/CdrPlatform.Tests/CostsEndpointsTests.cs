using CdrPlatform.Abstractions;
using CdrPlatform.Endpoints;
using CdrPlatform.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace CdrPlatform.Tests;

public class CostsEndpointsTests
{
    private readonly List<CallDetailRecord> _validCallDetailRecords =
    [
        new()
        {
            Id = 1,
            CallerId = 441215598896,
            Recipient = 448000096481,
            CallDate = new DateOnly(2023, 10, 1),
            EndTime = new TimeSpan(12, 0, 0),
            Duration = 60,
            Cost = 1.50m,
            Reference = "C5DA9724701EEBBA95CA2CC5617BA93E4",
            Currency = Currency.GBP
        },

        new()
        {
            Id = 2,
            CallerId = 442036401115,
            Recipient = 448004960493,
            CallDate = new DateOnly(2023, 10, 2),
            EndTime = new TimeSpan(13, 0, 0),
            Duration = 120,
            Cost = 3.00m,
            Reference = "C50B5A7BDB8D68B8512BB14A9D363CAA1",
            Currency = Currency.Unknown
        }
    ];
    
    [Fact]
    public async Task GetCostsByCallerAndMonthAsync_NoMatchingRecords_ReturnsNotFound()
    {
        // Arrange
        var dbContext = new MockDb().CreateDbContext();
        dbContext.CallDetailRecords.AddRange(_validCallDetailRecords);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await CostsEndpoints.GetCostsByCallerAndMonthAsync(6666, 2023, 10, dbContext, NullLogger.Instance);

        // Assert
        Assert.IsType<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>>(result);

        var notFoundResult = (NotFound)result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetCostsByCallerAndMonthAsync_MatchingRecords_ReturnsFilteredCosts()
    {
        // Arrange
        var dbContext = new MockDb().CreateDbContext();
        dbContext.CallDetailRecords.AddRange(_validCallDetailRecords);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await CostsEndpoints.GetCostsByCallerAndMonthAsync(441215598896, 2023, 10, dbContext, NullLogger.Instance);

        // Assert
        Assert.IsType<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>>(result);

        var okResult = (Ok<List<decimal>>)result.Result;
        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Single(okResult.Value);

        var cost = okResult.Value.First();
        Assert.Equal(1.50m, cost);
    }

    [Fact]
    public async Task GetCostsByCallerAndYearAsync_NoMatchingRecords_ReturnsNotFound()
    {
        // Arrange
        var dbContext = new MockDb().CreateDbContext();
        dbContext.CallDetailRecords.AddRange(_validCallDetailRecords);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await CostsEndpoints.GetCostsByCallerAndYearAsync(6666, 2023, dbContext, NullLogger.Instance);

        // Assert
        Assert.IsType<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>>(result);

        var notFoundResult = (NotFound)result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetCostsByCallerAndYearAsync_MatchingRecords_ReturnsFilteredCosts()
    {
        // Arrange
        var dbContext = new MockDb().CreateDbContext();
        dbContext.CallDetailRecords.AddRange(_validCallDetailRecords);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await CostsEndpoints.GetCostsByCallerAndYearAsync(441215598896, 2023, dbContext, NullLogger.Instance);

        // Assert
        Assert.IsType<Results<Ok<List<decimal>>, ProblemHttpResult, NotFound>>(result);

        var okResult = (Ok<List<decimal>>)result.Result;
        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
        Assert.Single(okResult.Value);

        var cost = okResult.Value.First();
        Assert.Equal(1.50m, cost);
    }
}