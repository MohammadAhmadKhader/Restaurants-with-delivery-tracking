using Auth.Dtos;
using Auth.Dtos.Auth;
using Auth.Dtos.User;
using Auth.Extensions.FluentValidationValidators;
using FluentValidation;

namespace Auth.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
        services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();
        services.AddScoped<IValidator<UsersFilterParams>, UserFilterParamsValidator>();
        
        return services;
    }
}