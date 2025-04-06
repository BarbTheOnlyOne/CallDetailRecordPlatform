using CdrPlatform.Abstractions;

namespace CdrPlatform.Dtos;

/// <summary>
/// Dto for inserting/updating the call detail record.
/// </summary>
/// <param name="CallerId">Phone number if the caller including the country code (i.e. 420 for CZ).</param>
/// <param name="Recipient">Phone number of the recipient including the country code (i.e. 420 for CZ).</param>
/// <param name="CallDate">Date at which the call was done.</param>
/// <param name="EndTime">Time of the end of the call.</param>
/// <param name="Duration">How long did the call take (in seconds).</param>
/// <param name="Cost">Price for the call.</param>
/// <param name="Reference">Reference ID.</param>
/// <param name="Currency">Currency of the price used for the call.</param>
public record UpsertCallDetailRecordDto(int CallerId, int Recipient, DateOnly CallDate, TimeSpan EndTime, int Duration, 
    decimal Cost, string Reference, Currency Currency);