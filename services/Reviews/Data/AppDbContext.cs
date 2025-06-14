using Microsoft.EntityFrameworkCore;
using Reviews.Models;

namespace Reviews.Data;
public class AppDbContext : DbContext
{
    public DbSet<RestaurantReview> RestaurantReviews { get; set; }
    public DbSet<MenuItemReview> MenuItemReviews { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}