using System.Text.Json;
using System.Text.Json.Serialization;

namespace KanjiReader.Domain.Common;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static JsonDefaults()
    {
        Options.Converters.Add(new JsonStringEnumConverter());
    }
}
