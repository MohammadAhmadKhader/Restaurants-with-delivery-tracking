namespace Shared.Tenant;
public class MockTenantProvider : ITenantProvider
{
    public Guid? RestaurantId => Guid.Parse(GetTenantId());
    public bool SkipTenantEnforcementOnCreate { get; set; }
    public string GetTenantId() => "default-tenant-id";
    public Guid GetTenantIdOrThrow() => Guid.NewGuid();
    public void SetTenantId(Guid? tenantId) { }
}