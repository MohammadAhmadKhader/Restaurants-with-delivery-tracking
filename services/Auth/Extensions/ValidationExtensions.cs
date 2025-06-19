using Auth.Contracts.Dtos.Address;
using Auth.Contracts.Dtos.Auth;
using Auth.Contracts.Dtos.Role;
using Auth.Contracts.Dtos.User;
using Auth.Extensions.FluentValidationValidators;
using FluentValidation;
using Shared.Extensions;

namespace Auth.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
        services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();
        services.AddScoped<IValidator<UsersFilterParams>, UserFilterParamsValidator>();
        services.AddScoped<IValidator<UserUpdateProfile>, UserUpdateProfileValidator>();
        services.AddScoped<IValidator<RoleCreateDto>, RoleCreateDtoValidator>();
        services.AddScoped<IValidator<RoleUpdateDto>, RoleUpdateDtoValidator>();
        services.AddScoped<IValidator<AddressCreateDto>, AddressCreateDtoValidator>();
        services.AddScoped<IValidator<AddressUpdateDto>, AddressUpdateDtoValidator>();

        ValidatorOptions.Global.ApplyDefaultConfigurations();
        
        return services;
    }
}