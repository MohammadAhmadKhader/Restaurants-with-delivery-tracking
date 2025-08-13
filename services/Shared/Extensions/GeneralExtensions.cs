using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Data.Patterns.GenericRepository;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Observability;
using Shared.Utils;
using Shared.Validation.FluentValidation;

namespace Shared.Extensions;

public static class GeneralExtensions
{
    public static IServiceCollection AddConventionalApplicationServices<TProgram, TDbContext>(
        this IServiceCollection services,
        bool applyDefaultValidatorOptions = true,
        bool addGenericRepository = true)
        where TProgram : class
        where TDbContext : DbContext
    {
        if (addGenericRepository)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<TProgram>()
                .AddClasses(classes => classes.Where(type =>
                    type.IsGenericTypeDefinition &&
                    type.Name == typeof(GenericRepository<,,>).Name))
                .As(typeof(IGenericRepository<,>))
                .WithScopedLifetime());
        }

        services.AddScoped<IUnitOfWork<TDbContext>, UnitOfWork<TDbContext>>();

        services.Scan(scan => scan
            .FromAssemblyOf<TProgram>()
            .AddClasses(classes => classes.Where(t =>
                t.Name.EndsWith("Service") ||
                t.Name.EndsWith("Repository") ||
                t.Name.EndsWith("Provider") ||
                t.Name.EndsWith("ServiceClient") || // for Refit clients
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
            ), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblyOf<TProgram>()
            .AddClasses(classes => classes.Where(t =>
                t.Name.EndsWith("Seeder")
            ), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        if (applyDefaultValidatorOptions)
        {
            ValidatorOptions.Global.ApplyDefaultConfigurations();
        }

        return services;
    }

    public static IConfigurationBuilder AddGlobalConfig(
        this IConfigurationBuilder cfg,
        bool optional = true,
        bool globalOptional = true,
        bool reloadOnChange = true,
        Action<IConfigurationBuilder>? configure = null)
    {
        var basePath = AppContext.BaseDirectory;
        var env = EnvironmentUtils.GetEnvName();

        cfg.AddJsonFile(Path.Combine(basePath, "globalsettings.json"), optional: globalOptional, reloadOnChange: reloadOnChange)
           .AddJsonFile(Path.Combine(basePath, $"globalsettings.{env}.json"), optional: globalOptional, reloadOnChange: reloadOnChange)
           .AddJsonFile(Path.Combine(basePath, "appsettings.json"), optional: optional, reloadOnChange: reloadOnChange)
           .AddJsonFile(Path.Combine(basePath, $"appsettings.{env}.json"), optional: optional, reloadOnChange: reloadOnChange)
           .AddEnvironmentVariables();

        configure?.Invoke(cfg);

        return cfg;
    }

    public static IServiceCollection AddAppServiceDefaults(
        this IServiceCollection services,
        ConfigurationManager configManager,
        ConfigureHostBuilder host,
        bool addGlobalConfig = true,
        bool addControllers = true,
        bool addNamingPolicy = true,
        bool addProblemDetails = true,
        bool addLogging = true,
        bool validateScopes = true)
    {
        if (addGlobalConfig)
        {
            configManager.AddGlobalConfig();
        }

        if (addControllers)
        {
            services.AddControllers();
        }

        if (addNamingPolicy)
        {
            services.AddNamingPolicy();
        }

        if (addProblemDetails)
        {
            services.AddAppProblemDetails();
        }

        if (addLogging)
        {
            services.AddServiceLogging(host, configManager);
        }

        if (validateScopes)
        {
            host.ValidateScopes();
        }

        return services;
    }
}