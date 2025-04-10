using CdrPlatform.Abstractions;
using CdrPlatform.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CdrPlatform.Services;

public sealed class CallDetailRecordMap : ClassMap<CallDetailRecord>
{
    // NOTE: Having these stupid defaults id BAD. Proper logic how to handle this should be implemented - skipping, rejecting, whatever.
    public CallDetailRecordMap()
    {
        Map(m => m.CallerId).Name("caller_id").Default(0);
        Map(m => m.Recipient).Name("recipient").Default(0);
        Map(m => m.CallDate).Name("call_date").TypeConverterOption.Format("dd/MM/yyyy").Default(new DateOnly());
        Map(m => m.EndTime).Name("end_time").TypeConverter(new CustomTimeSpanConverter()).Default(TimeSpan.Zero);
        Map(m => m.Duration).Name("duration").Default(0);
        Map(m => m.Cost).Name("cost").Default(0);
        Map(m => m.Reference).Name("reference").Default(string.Empty);
        Map(m => m.Currency).Name("currency").TypeConverter<EnumConverter<Currency>>().Default(Currency.Unknown);
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