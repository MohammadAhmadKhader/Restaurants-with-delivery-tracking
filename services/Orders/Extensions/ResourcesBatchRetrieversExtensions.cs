using Restaurants.Contracts.Clients;
using Restaurants.Contracts.Dtos.MenuItems;
using Shared.Common;

namespace Orders.Extensions;
public static class ResourcesBatchRetrieversExtensions
{
    public static IServiceCollection AddResourcesBatchRetrievers(this IServiceCollection services)
    {
        services.AddSingleton<IResourceBatchRetriever<int, MenuItemViewDto>>(provider =>
        {
            var menuClient = provider.GetRequiredService<IMenusServiceClient>();
            return new ResourceBatchRetriever<int, MenuItemViewDto>(
                async ids => (await menuClient.GetMenuItemsByIdAsync(ids.ToList())).Items,
                m => m.Id);
        });
    
        return services;
    }
}