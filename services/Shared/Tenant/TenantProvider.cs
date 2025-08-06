using Shared.Constants;

namespace Shared.Tenant;

public class TenantProvider : ITenantProvider
{
    public Guid? RestaurantId { get; private set; }
    public void SetTenantId(Guid? tenantId) => RestaurantId = tenantId;
    public Guid GetTenantIdOrThrow()
    {
        if (RestaurantId == null || RestaurantId == Guid.Empty)
        {
            throw new InvalidOperationException($"Missing or invalid {CustomHeaders.TenantHeader} header.");
        }

        return RestaurantId.Value;
    }
    
    public bool SkipTenantEnforcementOnCreate { get; set; } = false;
}