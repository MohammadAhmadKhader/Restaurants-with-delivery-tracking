using Auth.Dtos;
using Auth.Extensions.FluentValidation.Validators;
using FluentValidation;

namespace Auth.Extensions.FluentValidation;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
        return services;
    }
}