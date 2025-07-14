using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Shared.Utils;
public static class GuardUtils
{
    public static void ThrowIfNull<T>([NotNull] T arg, [CallerArgumentExpression(nameof(arg))] string? paramName = null)
    {
        if (arg == null)
        {
            throw new ArgumentNullException(paramName, "This argument can't be null.");
        }
    }
    public static void ThrowIfNull([NotNull] Guid? guid, [CallerArgumentExpression(nameof(guid))] string? paramName = null)
    {
        if (guid == null)
        {
            throw new ArgumentNullException(paramName, "The GUID can't be null.");
        }   
    }

    public static void ThrowIfEmpty(Guid guid, [CallerArgumentExpression(nameof(guid))] string? paramName = null)
    {
        if (guid == Guid.Empty)
        {
            throw new ArgumentException("The GUID can't be empty.", paramName);  
        }
            
    }

    public static void ThrowIfNullOrEmpty([NotNull] Guid? guid, [CallerArgumentExpression(nameof(guid))] string? paramName = null)
    {
        if (guid == null)
        {
            throw new ArgumentNullException(paramName, "The GUID can't be null.");
        }
        else if (guid == Guid.Empty)
        {
            throw new ArgumentException("The GUID can't be empty.", paramName);
        }
    }
}