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
public class CallDetailRecord
{
    public int Id { get; set; }
    public int CallerId { get; set; }
    public int Recipient { get; set; }
    public DateOnly CallDate { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration { get; set; }
    public decimal Cost { get; set; }
    public string Reference { get; set; }
    public Currency Currency { get; set; }
}