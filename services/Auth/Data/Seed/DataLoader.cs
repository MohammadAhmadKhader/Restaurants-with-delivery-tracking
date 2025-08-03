using System.Text.Json;

namespace Auth.Data.Seed;
public static class DataLoader
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    public static async Task<SeedDataModel> GetSeedData()
    {
        var json = await File.ReadAllTextAsync("./Data/seed.json");
        var seedData = JsonSerializer.Deserialize<SeedDataModel>(json, jsonOptions);
        if (seedData == null)
        {
            throw new InvalidOperationException("Failed to deserialize seed.json into SeedDataModel.");
        }

        return seedData;
    }
}