using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Filters;
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next(context);
        }

        var entity = context.Arguments.OfType<T>().FirstOrDefault();
        if (entity is null)
        {
            return Results.BadRequest("Invalid request payload.");
        }

        var validationResult = await validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new { field = ToCamelCase(e.PropertyName), message = e.ErrorMessage });
            return Results.BadRequest(new { errors });
        }

        return await next(context);
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}