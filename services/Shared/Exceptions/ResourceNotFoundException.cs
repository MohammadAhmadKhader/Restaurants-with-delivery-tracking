namespace Shared.Exceptions;

public class ResourceNotFoundException : Exception
{
    public string ResourceName { get; set; } = default!;
    public object? Id { get; set; }
    public static string ErrorMessageFormmatter(string resourceName) => $"{resourceName} was not found";
    public static string ErrorMessageWithIdFormmatter(string resourceName, object id) => $"{resourceName} with id '{id}' was not found";

    public ResourceNotFoundException(string resourceName) : base(ErrorMessageFormmatter(resourceName))
    {
        ResourceName = resourceName;
    }

    public ResourceNotFoundException(string resourceName, object id) : base(ErrorMessageWithIdFormmatter(resourceName, id))
    {
        ResourceName = resourceName;
        Id = id;
    }
    
    public static void ThrowIfNull(object? resource, string resourceName, object? id = null)
    {
        if (resource == null)
        {
            if (id != null)
            {
                throw new ResourceNotFoundException(resourceName, id);
            }

            throw new ResourceNotFoundException(resourceName);
        }
    }
}