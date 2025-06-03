using System.Net;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions;

public class ResourceNotFoundException : Exception
{
    public string ResourceName { get; set; } = default!;
    public object? Id { get; set; }
    
    public ResourceNotFoundException(string resourceName) : base($"{resourceName} was not found")
    {
        ResourceName = resourceName;
    }

    public ResourceNotFoundException(string resourceName, object id) : base($"{resourceName} with id '{id}' was not found")
    {
        ResourceName = resourceName;
        Id = id;
    }
}