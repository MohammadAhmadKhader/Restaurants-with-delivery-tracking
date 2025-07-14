namespace Shared.Utils;

public static class EnvironmentUtils
{
    private readonly static string testingString = "Testing";
    private readonly static string developmentString = "Development";
    private readonly static string productionString = "Production";
    private readonly static List<string> ignoreKafkaFlags = ["--no-kafka", "-nk"];
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

    public static bool IsKafkaSetToIgnore()
    {
        var args = Environment.GetCommandLineArgs();
        if (args.Contains(ignoreKafkaFlags[0]) || args.Contains(ignoreKafkaFlags[1]))
        {
            return true;
        }

        return false;
    }
    
    public static bool ShouldIgnoreKafka()
    {
        return IsKafkaSetToIgnore() || IsSeeding() || IsTesting();
    }
}