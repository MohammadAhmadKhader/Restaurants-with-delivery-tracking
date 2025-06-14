using FluentValidation;
using Shared.Extensions;

namespace Locations.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.ApplyDefaultConfigurations();
        
        return services;
    }
}