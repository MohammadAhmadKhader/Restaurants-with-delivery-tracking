using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Data.Patterns.GenericRepository;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Utils;

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

    /// <summary>
    /// Custom AddRange
    /// </summary>
    public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            target.Add(item);
        }
    }

    public static void AddRangeIf<T>(this ICollection<T> target, IEnumerable<T> items, Func<T, bool> conditionFunc)
    {

        foreach (var item in items)
        {
            if (conditionFunc(item))
            {
                target.Add(item);
            }
        }
    }

    /// <summary>
    /// Custom AddRange
    /// </summary>
    public static void AddRange<T>(this List<T> target, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}