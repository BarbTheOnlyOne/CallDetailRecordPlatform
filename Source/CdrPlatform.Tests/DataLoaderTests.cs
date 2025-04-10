using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CdrPlatform.Abstractions;
using CdrPlatform.Database;
using CdrPlatform.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CdrPlatform.Tests;

public class DataLoaderTests
{
    private readonly CdrDbContext _mockContext;
    private readonly CsvDataLoader _dataLoader;
    
    private const string CsvContent = "caller_id,recipient,call_date,end_time,duration,cost,reference,currency\n441215598896,448000096481,16/08/2016,14:21:33,43,0,C5DA9724701EEBBA95CA2CC5617BA93E4,GBP\n442036401149,44800833833,16/08/2016,14:00:47,244,0,C50B5A7BDB8D68B8512BB14A9D363CAA1,GBP";

    public DataLoaderTests()
    {
        // Setup mock context
        _mockContext = new MockDb().CreateDbContext();
        
        _dataLoader = new CsvDataLoader(_mockContext, NullLogger<CsvDataLoader>.Instance);
    }

    [Fact]
    public async Task LoadCsvToDbAsync_ValidCsvFile_AddsRecords()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + Guid.NewGuid() + ".csv";
        await File.WriteAllTextAsync(tempFile, CsvContent);
        
        // Act
        await _dataLoader.LoadCsvToDbAsync(tempFile);

        // Assert
        var count = await _mockContext.CallDetailRecords.CountAsync();
        Assert.Equal(2, count);

        var record = await _mockContext.CallDetailRecords.FindAsync(1);
        Assert.NotNull(record);
        Assert.Multiple(() =>
        {
            Assert.Equal(441215598896, record.CallerId);
            Assert.Equal(448000096481, record.Recipient);
            Assert.Equal(new DateOnly(2016, 8, 16), record.CallDate);
            Assert.Equal(new TimeSpan(14, 21, 33), record.EndTime);
            Assert.Equal(43, record.Duration);
            Assert.Equal(0m, record.Cost);
            Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record.Reference);
            Assert.Equal(Currency.GBP, record.Currency);
        });
        
        File.Delete(tempFile);
    }
    
    [Fact]
    public async Task LoadCsvToDbAsync_InvalidCsvFile_ReturnsFalseWithMessage()
    {
        // Arrange
        var invalidFile = "nonexistent.csv";
    
        // Act
        var result = await _dataLoader.LoadCsvToDbAsync(invalidFile);
        
        // Assert
        Assert.False(result.success);
        Assert.NotEmpty(result.error);
    }

    // TODO: To make this test happen, I would need to separe the DbContext from the DataLoader - create some kind of a mockable service
    // [Fact]
    // public async Task LoadCsvToDbBulkAsync_ValidCsvFile_AddsRecords()
    // {
    //     // Arrange
    //     var tempFile = Path.GetTempFileName() + Guid.NewGuid() + ".csv";
    //     await File.WriteAllTextAsync(tempFile, CsvContent);
    //
    //     // Act
    //     await _dataLoader.LoadCsvToDbBulkAsync(tempFile);
    //
    //     // Assert
    //     var count = await _mockContext.CallDetailRecords.CountAsync();
    //     Assert.Equal(2, count);
    //
    //     var record = await _mockContext.CallDetailRecords.FindAsync(1);
    //     Assert.NotNull(record);
    //     Assert.Multiple(() =>
    //     {
    //         Assert.Equal(441215598896, record.CallerId);
    //         Assert.Equal(448000096481, record.Recipient);
    //         Assert.Equal(new DateOnly(2016, 8, 16), record.CallDate);
    //         Assert.Equal(new TimeSpan(14, 21, 33), record.EndTime);
    //         Assert.Equal(43, record.Duration);
    //         Assert.Equal(0m, record.Cost);
    //         Assert.Equal("C5DA9724701EEBBA95CA2CC5617BA93E4", record.Reference);
    //         Assert.Equal(Currency.GBP, record.Currency);
    //     });
    //     
    //     File.Delete(tempFile);
    // }
    
    [Fact]
    public async Task LoadCsvToDbBulkAsync_InvalidCsvFile_ReturnsFalseWithMessage()
    {
        // Arrange
        var invalidFile = "nonexistent.csv";
    
        // Act
        var result = await _dataLoader.LoadCsvToDbBulkAsync(invalidFile);
        
        // Assert
        Assert.False(result.success);
        Assert.NotEmpty(result.error);
    }
}