using System.Collections.Concurrent;
using System.Reflection;
using Shared.Contracts.Attributes;

namespace Shared.Observability.Telemetry;

/// <summary>
/// ThisTelemetryManager purpose is to reduce the performance overhead from using reflection by caching the reflection data.
/// </summary>
public static class TelemetryManager
{
    private static readonly ConcurrentDictionary<PropertyInfo, bool> _maskPropertyCache = new();
    private static readonly ConcurrentDictionary<(MethodInfo, string), bool> _maskParamCache = new();
    private static readonly ConcurrentDictionary<MethodInfo, ParameterInfo[]> _parametersCache = new();
    private static readonly ConcurrentDictionary<Type, string> _dbSystemCache = new();
    private static readonly ConcurrentDictionary<MethodInfo, string> _activityNameCache = new();
    private static readonly ConcurrentDictionary<MethodInfo, string[]> _parameterNamesCache = new();
    private static readonly ConcurrentDictionary<(MethodInfo, int), bool> _isClassCache = new();

    public const string MaskValue = "******";

    public static bool IsPropertyMasked(PropertyInfo property) =>
            _maskPropertyCache.GetOrAdd(property, p => Attribute.IsDefined(p, typeof(MaskedAttribute)));

    public static bool IsParamMasked(MethodInfo method, string? paramName)
    {
        if (paramName == null) return false;

        return _maskParamCache.GetOrAdd((method, paramName), key =>
        {
            var (m, name) = key;
            var param = m.GetParameters().FirstOrDefault(p => p.Name == name);
            return param != null && Attribute.IsDefined(param, typeof(MaskedAttribute));
        });
    }

    public static ParameterInfo[] GetParameters(MethodInfo method)
        => _parametersCache.GetOrAdd(method, m => m.GetParameters());

    public static string GetDbSystem(Type? connectionType)
    {
        if (connectionType == null) return "Unknown";
        return _dbSystemCache.GetOrAdd(connectionType, t => t.Name);
    }

    public static string GetActivityName(MethodInfo method, Type targetType)
        => _activityNameCache.GetOrAdd(method, m => $"{targetType.Name}.{m.Name}");

    public static string[] GetParametersNames(MethodInfo method)
        => _parameterNamesCache.GetOrAdd(method, m => m.GetParameters().Select(p => p.Name!).ToArray());

    public static bool IsClass(MethodInfo method, int paramIndex, object? argValue)
    {
        if (argValue == null) return false;
        return _isClassCache.GetOrAdd((method, paramIndex), _ => argValue.GetType().IsClass);
    }
}