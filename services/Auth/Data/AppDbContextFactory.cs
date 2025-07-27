using Auth.Utils;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shared.Extensions;

namespace Auth.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        Env.Load();

        var config = new ConfigurationBuilder()
            .AddGlobalConfig()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

        return new AppDbContext(optionsBuilder.Options, new MockTenantProvider());
    }
    
    public class MockTenantProvider : ITenantProvider
    {
        public Guid? RestaurantId => Guid.Parse(GetTenantId());
        public string GetTenantId() => "default-tenant-id";
        public Guid GetTenantIdOrThrow() => Guid.NewGuid();
        public void SetTenantId(Guid? tenantId) { }
    }
}