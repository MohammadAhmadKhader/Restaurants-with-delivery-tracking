using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(b =>
        {
            b.Property(u => u.Email).HasMaxLength(64);
            b.Property(u => u.PasswordHash).HasMaxLength(56);
            b.Property(u => u.FirstName).HasMaxLength(64);
            b.Property(u => u.LastName).HasMaxLength(64);

            b.ToTable(t => t.HasCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                sql: @"
                    is_deleted
                    OR (
                        email             IS NOT NULL
                        AND password_hash IS NOT NULL
                        AND first_name    IS NOT NULL
                        AND last_name     IS NOT NULL
                    )"
                ));
        });

        modelBuilder.Entity<IdentityRole>(b =>
        {
            b.Property(r => r.Name).HasMaxLength(256).IsRequired();
            b.Property(r => r.NormalizedName).HasMaxLength(256);
        });
    }
}