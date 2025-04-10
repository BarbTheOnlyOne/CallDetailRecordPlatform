using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CdrPlatform.Services;

public class EnumConverter<T> : DefaultTypeConverter where T : struct
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return Enum.TryParse<T>(text, true, out var result) ? result : default;
    }
}