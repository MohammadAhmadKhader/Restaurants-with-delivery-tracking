using Microsoft.EntityFrameworkCore;
using Orders.Contracts.Enums;
using Orders.Models;
using Orders.Utils;
using Shared.Tenant;
namespace Orders.Data;

public class AppDbContext : DbContext
{
    public readonly ITenantProvider _tenantProvider;
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>().HasQueryFilter(o =>
        _tenantProvider.RestaurantId != null && o.RestaurantId == _tenantProvider.RestaurantId);

        var orderStatusTypeName = "orderstatus";
        modelBuilder.HasPostgresEnum<OrderStatus>(name: orderStatusTypeName);
        modelBuilder.Entity<Order>(o =>
        {
            o.Property(o => o.DeliveryTrackingEnabled).HasDefaultValue(true);
            o.Property(o => o.Status)
                .HasColumnType(orderStatusTypeName)
                .HasDefaultValueSql($"'{OrderStatus.Placed.ToString().ToLower()}'::{orderStatusTypeName}");
        });

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(o => o.OrderId)
            .IsRequired();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantGuard();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantGuard()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantModel tenantModel &&
                entry.State == EntityState.Added &&
                !_tenantProvider.SkipTenantEnforcementOnCreate)
            {
                tenantModel.RestaurantId = _tenantProvider.GetTenantIdOrThrow();
            }
        }
    }
}