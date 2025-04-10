using System.Globalization;
using CdrPlatform.Database;
using CdrPlatform.Endpoints;
using CdrPlatform.Models;
using CsvHelper;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CdrPlatform.Services;

public class CsvDataLoader(CdrDbContext context, ILogger<CsvDataLoader> logger)
{
    private readonly int _batchSize = 1000; // TODO: Make this configurable, otherwise constant would be better

    public async Task<(bool success, string error)> LoadCsvToDbAsync(string csvFilePath)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CallDetailRecordMap>();
                var records = csv.GetRecords<CallDetailRecord>().ToList();

                for (var i = 0; i < records.Count; i += _batchSize)
                {
                    var batch = records.Skip(i).Take(_batchSize).ToList();
                    context.CallDetailRecords.AddRange(batch);

                    // Save changes for this batch
                    await context.SaveChangesAsync();

                    // Detach entities to free memory
                    context.ChangeTracker.Clear();
                }
            }

            logger.LogInformation("Data loading completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading data from CSV");
            return (false, ex.Message);
        }
        
        return (true, string.Empty);
    }
    
    public async Task<(bool success, string error)> LoadCsvToDbBulkAsync(string csvFilePath)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();

            // Disable auto detect changes for better performance
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<CallDetailRecordMap>();
            var records = csv.GetRecords<CallDetailRecord>().ToList();

            await context.Database.BeginTransactionAsync();

            try
            {
                for (var i = 0; i < records.Count; i += _batchSize)
                {
                    var batch = records.Skip(i).Take(_batchSize).ToList();
                    await context.BulkInsertAsync(batch);
                    logger.LogInformation("Processed {count} records", i + batch.Count);
                }

                await context.Database.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await context.Database.RollbackTransactionAsync();
                logger.LogError(e, "Error during bulk insert");
                return (false, e.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading data from CSV");
            return (false, ex.Message);
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
        
        return (true, string.Empty);
    }
}