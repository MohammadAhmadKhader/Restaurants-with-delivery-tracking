namespace Shared.Tenant;
public interface ITenantProvider
{
    Guid? RestaurantId { get; }
    Guid GetTenantIdOrThrow();
    void SetTenantId(Guid? tenantId);

    // optional flag
    bool SkipTenantEnforcementOnCreate { get; set; }
}