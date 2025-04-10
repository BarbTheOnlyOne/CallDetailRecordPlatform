using CdrPlatform.Abstractions;
using CdrPlatform.Models;
using CsvHelper.Configuration;

namespace CdrPlatform.Services;

public sealed class CallDetailRecordMap : ClassMap<CallDetailRecord>
{
    public CallDetailRecordMap()
    {
        Map(m => m.CallerId).Name("caller_id");
        Map(m => m.Recipient).Name("recipient");
        Map(m => m.CallDate).Name("call_date").TypeConverterOption.Format("yyyy/MM/dd");
        Map(m => m.EndTime).Name("end_time").TypeConverterOption.Format("HH:mm:ss");
        Map(m => m.Duration).Name("duration");
        Map(m => m.Cost).Name("cost");
        Map(m => m.Reference).Name("reference");
        Map(m => m.Currency).Name("currency").TypeConverter<EnumConverter<Currency>>();
    }
}