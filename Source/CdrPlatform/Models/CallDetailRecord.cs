using CdrPlatform.Abstractions;

namespace CdrPlatform.Models;

/// <summary>
/// This class is used for storing the data about a single call.
/// 
/// 
/// NOTE: I was thinking about joining the call date & end time properties into a DateTime, since that seemed elegant 
/// type. But that would probably bring a lot of complications, since this would introduce unnecessary parsing logic 
/// and moreover, the DB can be kept the same and language could be switched - and this language might not have 
/// the DateTime type.
/// </summary>
/// <param name="CallerId">Phone number if the caller including the country code (i.e. 420 for CZ).</param>
/// <param name="Recipient">Phone number of the recipient including the country code (i.e. 420 for CZ).</param>
/// <param name="CallDate">Date at which the call was done.</param>
/// <param name="EndTime">Time of the end of the call.</param>
/// <param name="Duration">How long did the call take (in seconds).</param>
/// <param name="Cost">Price for the call.</param>
/// <param name="Reference">Reference ID.</param>
/// <param name="Currency">Currency of the price used for the call.</param>
public record CallDetailRecord(int CallerId, int Recipient, DateOnly CallDate, TimeSpan EndTime, int Duration, 
    decimal Cost, string Reference, Currency Currency);