using System.Runtime.CompilerServices;
using Auth.Models;
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

        var currDateSqlStatement = "NOW() AT TIME ZONE 'UTC'";
        modelBuilder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql(currDateSqlStatement);
        modelBuilder.Entity<User>().Property(u => u.UpdatedAt).HasDefaultValueSql(currDateSqlStatement);

        modelBuilder.Entity<User>()
        .Property(u => u.IsDeleted)
        .HasDefaultValue(false);

        // in the mean time we will assume users are always confirmed
        // might be changed later
        modelBuilder.Entity<User>()
        .Property(u => u.EmailConfirmed)
        .HasDefaultValue(true);

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
                        ""PasswordHash""        IS NULL AND
                        ""FirstName""           IS NULL AND
                        ""LastName""            IS NULL
                    )"
            ));
        });

        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>()
        .HasMany(u => u.Roles)
        .WithMany(r => r.Users);

        modelBuilder.Entity<Role>()
        .HasMany(u => u.Permissions)
        .WithMany(p => p.Roles);

        modelBuilder.Entity<User>()
        .HasMany(u => u.Addresses)
        .WithOne(a => a.User);
        
        modelBuilder.Entity<User>().Property(u => u.UpdatedAt)
        .ValueGeneratedOnAddOrUpdate();
    }
}