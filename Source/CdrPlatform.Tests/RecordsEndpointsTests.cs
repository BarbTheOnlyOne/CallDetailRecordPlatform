using CdrPlatform.Abstractions;
using CdrPlatform.Endpoints;
using CdrPlatform.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace CdrPlatform.Tests
{
    public class RecordsEndpointsTests
    {
        public List<CallDetailRecord> ValidCallDetailRecords = new List<CallDetailRecord>
        {
            new CallDetailRecord
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
            new CallDetailRecord
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
        };
        
        [Fact]
        public void GetRecordByReferenceAsync_RecordNotFound_ReturnsNotFound()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);

            // Act
            var result = RecordsEndpoints.GetRecordByReferenceAsync("NONEXISTENTREFERENCE123", dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<CallDetailRecord>, ProblemHttpResult, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task GetRecordByReferenceAsync_ValidReference_ReturnsSingleRecord()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();

            // Act
            var result = RecordsEndpoints.GetRecordByReferenceAsync("C5DA9724701EEBBA95CA2CC5617BA93E4", dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<CallDetailRecord>, ProblemHttpResult, NotFound>>(result);

            var okResult = (Ok<CallDetailRecord>)result.Result;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);

            var record = okResult.Value;
            Assert.Multiple(() =>
            {
                Assert.Equal(1, record.Id);
                Assert.Equal(441215598896, record.CallerId);
                Assert.Equal(448000096481, record.Recipient);
                Assert.Equal(new DateOnly(2023, 10, 1), record.CallDate);
                Assert.Equal(new TimeSpan(12, 0, 0), record.EndTime);
                Assert.Equal(60, record.Duration);
                Assert.Equal(1.50m, record.Cost);
                Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record.Reference);
                Assert.Equal(Currency.GBP, record.Currency);
            });
        }
        
        [Fact]
        public async Task GetRecordsAsync_EmptyDatabase_ReturnsNotFound()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            

            // Act
            var result = await RecordsEndpoints.GetRecordsAsync(dbContext, NullLogger.Instance);
            
            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;
            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task GetRecordsAsync_PopulatedDatabase_ReturnsAllRecords()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await RecordsEndpoints.GetRecordsAsync(dbContext, NullLogger.Instance);
            
            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var okResult = (Ok<List<CallDetailRecord>>)result.Result;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
            Assert.Equal(2, okResult.Value.Count);
            Assert.Collection(okResult.Value, record1 =>
            {
                Assert.Equal(1, record1.Id);
                Assert.Equal(441215598896, record1.CallerId);
                Assert.Equal(448000096481, record1.Recipient);
                Assert.Equal(new DateOnly(2023, 10, 1), record1.CallDate);
                Assert.Equal(new TimeSpan(12, 0, 0), record1.EndTime);
                Assert.Equal(60, record1.Duration);
                Assert.Equal(1.50m, record1.Cost);
                Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record1.Reference);
                Assert.Equal(Currency.GBP, record1.Currency);
            }, record2 =>
            {
                Assert.Equal(2, record2.Id);
                Assert.Equal(442036401115, record2.CallerId);
                Assert.Equal(448004960493, record2.Recipient);
                Assert.Equal(new DateOnly(2023, 10, 2), record2.CallDate);
                Assert.Equal(new TimeSpan(13, 0, 0), record2.EndTime);
                Assert.Equal(120, record2.Duration);
                Assert.Equal(3.00m, record2.Cost);
                Assert.Equal("C50B5A7BDB8D68B8512BB14A9D363CAA1", record2.Reference);
                Assert.Equal(Currency.Unknown, record2.Currency);
            });
        }

        [Fact]
        public async Task GetRecordsByCallerAsync_NoMatchingRecords_ReturnsNotFound()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await RecordsEndpoints.GetRecordsByCallerAsync(6666, dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;
            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task GetRecordsByCallerAsync_MatchingRecords_ReturnsFilteredRecords()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();
            // Act
            var result = await RecordsEndpoints.GetRecordsByCallerAsync(441215598896, dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var okResult = (Ok<List<CallDetailRecord>>)result.Result;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Single(okResult.Value);

            var record = okResult.Value.First();
            Assert.Equal(1, record.Id);
            Assert.Equal(441215598896, record.CallerId);
            Assert.Equal(448000096481, record.Recipient);
            Assert.Equal(new DateOnly(2023, 10, 1), record.CallDate);
            Assert.Equal(new TimeSpan(12, 0, 0), record.EndTime);
            Assert.Equal(60, record.Duration);
            Assert.Equal(1.50m, record.Cost);
            Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record.Reference);
            Assert.Equal(Currency.GBP, record.Currency);
        }
        
        [Fact]
        public async Task GetRecordsByCallerAndMonthAsync_NoMatchingRecords_ReturnsNotFound()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await RecordsEndpoints.GetRecordsByCallerAndMonthAsync(441215598896, 2023, 11, dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task GetRecordsByCallerAndMonthAsync_MatchingRecords_ReturnsFilteredRecords()
        {
            // Arrange
            var dbContext = new MockDb().CreateDbContext();
            dbContext.CallDetailRecords.AddRange(ValidCallDetailRecords);
            await dbContext.SaveChangesAsync();
            // Act
            var result = await RecordsEndpoints.GetRecordsByCallerAndMonthAsync(441215598896, 2023, 10, dbContext, NullLogger.Instance);

            // Assert
            Assert.IsType<Results<Ok<List<CallDetailRecord>>, ProblemHttpResult, NotFound>>(result);

            var okResult = (Ok<List<CallDetailRecord>>)result.Result;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.Single(okResult.Value);

            var record = okResult.Value.First();
            Assert.Equal(1, record.Id);
            Assert.Equal(441215598896, record.CallerId);
            Assert.Equal(448000096481, record.Recipient);
            Assert.Equal(new DateOnly(2023, 10, 1), record.CallDate);
            Assert.Equal(new TimeSpan(12, 0, 0), record.EndTime);
            Assert.Equal(60, record.Duration);
            Assert.Equal(1.50m, record.Cost);
            Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record.Reference);
            Assert.Equal(Currency.GBP, record.Currency);
        }
    }
}