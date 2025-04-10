using CdrPlatform.Abstractions;
using CdrPlatform.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CdrPlatform.Services;

public sealed class CallDetailRecordMap : ClassMap<CallDetailRecord>
{
    public CallDetailRecordMap()
    {
        Map(m => m.CallerId).Name("caller_id");
        Map(m => m.Recipient).Name("recipient");
        Map(m => m.CallDate).Name("call_date").TypeConverterOption.Format("dd/MM/yyyy");
        Map(m => m.EndTime).Name("end_time").TypeConverter(new CustomTimeSpanConverter());
        Map(m => m.Duration).Name("duration");
        Map(m => m.Cost).Name("cost");
        Map(m => m.Reference).Name("reference");
        Map(m => m.Currency).Name("currency").TypeConverter<EnumConverter<Currency>>();
    }
    
    private class CustomTimeSpanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return TimeSpan.Zero;
            }
            
            if (TimeSpan.TryParse(text, out var result))
            {
                return result;
            }
            
            throw new Exception($"Cannot convert '{text}' to TimeSpan.");
        }
    }
}