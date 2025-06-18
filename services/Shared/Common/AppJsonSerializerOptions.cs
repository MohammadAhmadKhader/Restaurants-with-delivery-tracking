using System.Text.Json;

namespace Shared.Common;

public static class AppJsonSerializerOptions
{
    public static JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };
}
