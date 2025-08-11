using Microsoft.EntityFrameworkCore;
using Payments.Models;
using Shared.Extensions;
using Shared.Tenant;

namespace Payments.Data;
public class AppDbContext : DbContext
{   
    private readonly ITenantProvider _tenantProvider;
    public DbSet<Payment> Payments { get; set; }
    public DbSet<AppStripeCustomer> StripeCustomers { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppStripeCustomer>().HasQueryFilter(o =>
        _tenantProvider.RestaurantId != null && o.RestaurantId == _tenantProvider.RestaurantId);

        modelBuilder.Entity<AppStripeCustomer>(entity =>
        {
            entity.AddGuidNotEmptyConstraint(nameof(AppStripeCustomer.UserId));
            entity.AddGuidNotEmptyConstraint(nameof(AppStripeCustomer.RestaurantId));

            // since we have global filter on RestaurantId queries will filter by RestaurantId by default then by UserId
            // so the unique index has used the RestaurantId to improve searching
            // Note: UserId is unique even without being scoped to a Restaurant, but indexing with RestaurantId improves query performance
            entity.HasIndex(s => new { s.RestaurantId, s.UserId }).IsUnique();
            entity.HasIndex(s => s.StripeCustomerId).IsUnique();
        });
    }
}