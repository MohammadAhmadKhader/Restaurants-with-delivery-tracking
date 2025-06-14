using Locations.Models;
using Microsoft.EntityFrameworkCore;

namespace Locations.Data;
public class AppDbContext : DbContext
{
    public DbSet<LocationHistory> LocationsHistories { get; set; }
    public DbSet<CurrentLocation> CurrentLocations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}