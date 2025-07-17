namespace Shared.Utils;

public static class EnvironmentUtils
{
    private readonly static string testingString = "Testing";
    private readonly static string developmentString = "Development";
    private readonly static string productionString = "Production";
    private readonly static List<string> kafkaInMemoryFlags = ["--memory-kafka", "-mk"];
    public static string GetEnvName()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "No name provided";
    }
    public static bool IsTesting()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == testingString;
    }
    public static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == developmentString;
    }

    public static bool IsProduction()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == productionString;
    }

    public static bool IsSeeding()
    {
        var args = Environment.GetCommandLineArgs();
        return args.Contains("--seed");
    }

    /// <summary>
    /// used for fast testing in development (when you dont want to run the kubernetes cluster 
    /// and dont need it in development mode at a certain time)
    /// </summary>
    public static bool IsOnlyKafkaRunInMemory()
    {
        var args = Environment.GetCommandLineArgs();
        if (args.Contains(kafkaInMemoryFlags[0]) || args.Contains(kafkaInMemoryFlags[1]))
        {
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// This checks whether kafka is ignored or not
    /// </summary>
    public static bool ShouldIgnoreKafka()
    {
        return IsSeeding() || IsTesting();
    }
}