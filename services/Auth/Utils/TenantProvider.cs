namespace Auth.Utils;
public class TenantProvider : ITenantProvider
{
    public const string TenantHeader = "X-Restaurant-Id";
    public Guid? RestaurantId { get; private set; }
    public void SetTenantId(Guid? tenantId) => RestaurantId = tenantId;
    public Guid GetTenantIdOrThrow()
    {
        if (RestaurantId == null || RestaurantId == Guid.Empty)
        {
            throw new InvalidOperationException("Missing or invalid X-Restaurant-Id header.");
        }

        return RestaurantId.Value;
    }
}

public interface ITenantProvider
{
    Guid? RestaurantId { get; }
    Guid GetTenantIdOrThrow();
    void SetTenantId(Guid? tenantId);
}