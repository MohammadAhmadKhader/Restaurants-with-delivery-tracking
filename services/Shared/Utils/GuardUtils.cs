namespace Shared.Utils;
public static class GuardUtils
{
    public static void ThrowIfNull<T>(T arg, string? paramName = null)
    {
        if (arg == null)
        {
            throw new ArgumentNullException(paramName ?? nameof(arg), "This argument can't be null.");
        }
    }
    public static void ThrowIfNull(Guid? guid, string paramName = "id")
    {
        if (guid == null)
        {
            throw new ArgumentNullException(paramName, "The GUID can't be null.");
        }   
    }

    public static void ThrowIfEmpty(Guid guid, string paramName = "id")
    {
        if (guid == Guid.Empty)
        {
            throw new ArgumentException("The GUID can't be empty.", paramName);  
        }
            
    }

    public static void ThrowIfNullOrEmpty(Guid? guid, string paramName = "id")
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