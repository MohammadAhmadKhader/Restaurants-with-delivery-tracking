using Microsoft.EntityFrameworkCore;
using Restaurants.Models;
using Restaurants.Utils;
using Shared.Tenant;

namespace Restaurants.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantInvitation> RestaurantInvitations { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Menu>()
        .HasQueryFilter(m => m.RestaurantId == _tenantProvider.RestaurantId);

        modelBuilder.Entity<MenuItem>()
        .HasQueryFilter(m => m.RestaurantId == _tenantProvider.RestaurantId);

        modelBuilder.Entity<Menu>()
            .HasIndex(m => new { m.RestaurantId, m.NormalizedName })
            .IsUnique();
        modelBuilder.Entity<MenuItem>()
            .HasIndex(m => new { m.RestaurantId, m.NormalizedName })
            .IsUnique();

        modelBuilder.Entity<MenuItem>()
            .Property(i => i.IsAvailable)
            .HasDefaultValue("TRUE");

        modelBuilder.Entity<MenuItem>()
            .Property(x => x.Price)
            .HasPrecision(8, 2);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantGuard();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantGuard()
    {
        if (!_tenantProvider.SkipTenantEnforcementOnCreate)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantModel>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.RestaurantId = _tenantProvider.GetTenantIdOrThrow();
                }
            }
        }
    }
}