using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
// builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    var config = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(config))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");
    }
    options.UseNpgsql(config);
});


builder.Services.AddIdentity<User, IdentityRole<Guid>>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

app.MapAuthEndpoints();
app.Run();