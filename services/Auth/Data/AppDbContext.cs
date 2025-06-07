using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Address> Addresses { get; set; }

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

        modelBuilder.Entity<User>().ToTable("Users")
        .HasQueryFilter(u => !u.IsDeleted);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.Addresses)
            .WithOne(a => a.User);

            entity.HasIndex(r => r.NormalizedEmail).IsUnique();

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

        // * ----- Other Entities -----
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}