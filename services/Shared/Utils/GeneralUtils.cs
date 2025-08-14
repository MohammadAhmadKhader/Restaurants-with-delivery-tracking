using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Shared.Utils;

public static class GeneralUtils
{
    public static string CamelToPascal(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static string PascalToCamel(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToLower(input[0]) + input.Substring(1);
    }
    public static List<string> GetAllStringConstants(Type targetClass)
    {
        GuardUtils.ThrowIfNull(targetClass);
        if (!targetClass.IsClass)
        {
            throw new ArgumentException($"Type must be a class, not {targetClass.Name}.", nameof(targetClass));
        }

        return targetClass
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => (string)fi.GetRawConstantValue()!).ToList();
    }

    public static async Task LogOnErrorAsync(Func<Task> action, string? errorMessage = null)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Log.Error(ex, "An error occurred: {CustomMessage}", errorMessage);
            }
            else
            {
                Log.Error(ex, "An error occurred");
            }

            throw;
        }
    }

    public static string GetServiceName(bool lowercaseFirstLetter = false)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        GuardUtils.ThrowIfNull(assemblyName);
        
        var serviceName = assemblyName?.Name;
        GuardUtils.ThrowIfNull(serviceName);

        if (lowercaseFirstLetter)
        {
            return PascalToCamel(serviceName);
        }

        return serviceName;
    }

    public static string? GetServiceVersion<TProgram>()
        where TProgram: class
    {
        var assembly = typeof(TProgram).Assembly;
        var version = assembly.GetName().Version;
        
        return version?.ToString();
    }

    public static async Task ActionOnThrowAsync(Func<Task> func, Action actionOnThrow, bool shouldLog = true)
    {
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            if (shouldLog)
            {
                Log.Error(ex, "An error occurred");
            }

            actionOnThrow();
        }
    }

    public static Microsoft.Extensions.Logging.ILogger GetLogger(
        IServiceCollection services,
        // this is ignored in logs anyway
        [CallerMemberName] string callerName = ""
    )
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(callerName);

        return logger;
    }
}