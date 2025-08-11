using Shared.Exceptions;

namespace Shared.Common;

/// <summary>
/// This is used with service clients when you need to get many items by id's and want to check the presence of all the requested items
/// <para>
/// <code> 
/// services.AddSingleton&lt;IResourceBatchRetriever&lt;int, MenuItemViewDto&gt;&gt;(provider =>
/// {
///     var menuClient = provider.GetRequiredService&lt;IMenusServiceClient&gt;();
///     return new ResourceBatchRetriever&lt;int, MenuItemViewDto&gt;(
///         ids => menuClient.GetMenuItemsByIdAsync(ids),
///         m => m.Id);
/// });
/// 
///  var menuItems = await resourceBatchRetriever.ValidateAndRetrieveAsDictAsync(
///         dto.Items.Select(x => x.MenuItemId), "menu-item");
/// </code>
/// </para>
/// </summary>
/// <typeparam name="TKey">
/// The type of the resource key. Must be non-nullable (e.g., <c>Guid</c>, <c>int</c>, or <c>string</c>).
/// </typeparam>
/// <typeparam name="TResource">
/// The type of the resource being retrieved.
/// </typeparam>
/// <param name="serviceCall">
/// A function that takes a collection of keys and returns a task containing the resources mapped by their keys.
/// </param>
/// <param name="keySelector">
/// A function that selects the key from a resource object.
/// </param>
public interface IResourceBatchRetriever<TKey, TResource>
    where TKey : notnull
{
    Task<Dictionary<TKey, TResource>> ValidateAndRetrieveAsDictAsync(IEnumerable<TKey> ids, string resourceType);
    Task<List<TResource>> ValidateAndRetrieveAsListAsync(IEnumerable<TKey> ids, string resourceType);
}

public class ResourceBatchRetriever<TKey, TResource>(
    Func<IEnumerable<TKey>, Task<IEnumerable<TResource>>> serviceCall,
    Func<TResource, TKey> keySelector) : IResourceBatchRetriever<TKey, TResource>
    where TKey : notnull
{
    private readonly Func<IEnumerable<TKey>, Task<IEnumerable<TResource>>> _serviceCall = serviceCall;
    private readonly Func<TResource, TKey> _keySelector = keySelector;

    public async Task<Dictionary<TKey, TResource>> ValidateAndRetrieveAsDictAsync(
        IEnumerable<TKey> ids,
        string resourceType)
    {
        var requestedIds = ids.ToList();
        if (requestedIds.Count == 0)
        {
            throw new ArgumentException("At least one id is required.");
        }

        var items = await _serviceCall(requestedIds);
        var resourcesById = items.ToDictionary(_keySelector);

        if (items.Count() != requestedIds.Count)
        {
            var missingId = requestedIds.First(id => !resourcesById.ContainsKey(id));
            throw new ResourceNotFoundException(resourceType, missingId);
        }

        return resourcesById;
    }

    public async Task<List<TResource>> ValidateAndRetrieveAsListAsync(
        IEnumerable<TKey> ids,
        string resourceType)
    {
        var requestedIds = ids.ToList();
        if (requestedIds.Count == 0)
            throw new ArgumentException("At least one id is required.");

        var items = (await _serviceCall(requestedIds)).ToList();
        var itemKeys = items.Select(_keySelector).ToHashSet();

        if (itemKeys.Count != requestedIds.Count)
        {
            var missingId = requestedIds.First(id => !itemKeys.Contains(id));
            throw new ResourceNotFoundException(resourceType, missingId);
        }

        return items;
    }
}