using Microsoft.EntityFrameworkCore;
using Restaurants.Models;

namespace Restaurants.Data;
public class AppDbContext : DbContext
{
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantInvitation> RestaurantInvitations { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}