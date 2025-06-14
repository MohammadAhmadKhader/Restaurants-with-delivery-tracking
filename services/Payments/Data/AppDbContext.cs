using Microsoft.EntityFrameworkCore;
using Payments.Models;

namespace Payments.Data;
public class AppDbContext : DbContext
{   
    public DbSet<Payment> Payments { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}