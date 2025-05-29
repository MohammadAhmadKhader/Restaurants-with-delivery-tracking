using System.Text.Json;

namespace Shared.Utils;

public class SeedingUtils
{
    public static async Task<TData> ParseJson<TData>(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = await File.ReadAllTextAsync(filePath);
        var seedData = JsonSerializer.Deserialize<TData>(json, options);
        if (seedData == null)
        {
            throw new InvalidOperationException($"json file in path {filePath} was not found");
        }

        return seedData;
    }
}