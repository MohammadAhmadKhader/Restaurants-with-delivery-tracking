using Auth.Models;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    private readonly ITenantProvider _tenantProvider;
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<RestaurantRole> RestaurantRoles { get; set; }
    public DbSet<RestaurantPermission> RestaurantPermissions { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // * ----- Shared -----
        var currDateSqlStatement = "NOW() AT TIME ZONE 'UTC'";

        // * ----- User Entity -----
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable(t => t.HasCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                sql: @"
                    ""IsDeleted"" = FALSE OR (
                        ""Email""               IS NULL AND
                        ""UserName""            IS NULL AND
                        ""NormalizedUserName""  IS NULL AND
                        ""NormalizedEmail""     IS NULL AND
                        ""PhoneNumber""         IS NULL AND
                        ""PasswordHash""        IS NULL AND
                        ""FirstName""           IS NULL AND
                        ""LastName""            IS NULL
                    )"
            ));
        });

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable(t => t.HasCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                sql: @" 
                    ""IsGlobal"" = TRUE OR (
                        ""RestaurantId""  IS NULL
                    )"
            ));
        });

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<User>()
        .HasQueryFilter(u => !u.IsDeleted && u.RestaurantId == _tenantProvider.RestaurantId);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.Addresses)
            .WithOne(a => a.User);

            entity.HasIndex(u => new { u.RestaurantId, u.NormalizedEmail }).IsUnique();

            entity.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<Guid>>(
                l => l.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId),
                r => r.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId),
                ur =>
                {
                    ur.HasKey(x => new { x.UserId, x.RoleId });
                    ur.ToTable("UserRoles");
                }
            );

            entity.HasMany(u => u.RestaurantRoles)
            .WithMany(rr => rr.Users)
            .UsingEntity<UserRestaurantRole>(
                l => l.HasOne<RestaurantRole>().WithMany().HasForeignKey(urr => urr.RestaurantId),
                r => r.HasOne<User>().WithMany().HasForeignKey(urr => urr.UserId),
                urr =>
                {
                    urr.HasKey(x => new { x.UserId, x.RestaurantId });
                    urr.ToTable("UserRestaurantRoles");
                }
            );

            // in the mean time we will assume users are always confirmed
            // might be changed later
            entity.Property(u => u.EmailConfirmed)
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt).HasDefaultValueSql(currDateSqlStatement);
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql(currDateSqlStatement);
            entity.Property(u => u.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate();

            entity.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(u => u.IsGlobal)
                .HasDefaultValue(false);

            var defaultUserNameIndex = entity.Metadata
                .GetIndexes()
                .FirstOrDefault(i => i.GetDatabaseName() == "UserNameIndex");
            if (defaultUserNameIndex != null)
            {
                entity.Metadata.RemoveIndex(defaultUserNameIndex);
            }

            entity.HasIndex(u => new { u.RestaurantId, u.NormalizedUserName })
                .IsUnique()
                .HasDatabaseName("IX_Users_RestaurantId_NormalizedUserName");
        });

        // * ----- Role Entity -----
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasMany(u => u.Permissions)
                .WithMany(p => p.Roles);

            entity.HasIndex(r => r.NormalizedName).IsUnique();
        });

        // * ----- Permission Entity -----
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
        });

        // * ----- Restaurant Role Entity -----
        modelBuilder.Entity<RestaurantRole>().ToTable("RestaurantRoles");
        modelBuilder.Entity<RestaurantRole>()
        .HasQueryFilter(rr => rr.RestaurantId == _tenantProvider.RestaurantId);

        modelBuilder.Entity<RestaurantRole>(entity =>
        {
            entity.HasMany(u => u.Permissions)
                .WithMany(p => p.Roles);

            entity.HasIndex(r => r.NormalizedName).IsUnique();
        });

        // * ----- Restaurant Permission Entity -----
        modelBuilder.Entity<RestaurantPermission>().ToTable("RestaurantPermissions");
        modelBuilder.Entity<RestaurantPermission>(entity =>
        {
            entity.HasIndex(r => r.NormalizedName).IsUnique();
        });

        // * ----- Other Entities -----
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}