using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shared.Exceptions;

namespace Shared.Filters;
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            throw new InternalServerException($"Validatior type {typeof(IValidator<T>)} was not found");
        }

        var entity = context.Arguments.OfType<T>().FirstOrDefault();
        if (entity is null)
        {
            throw new InvalidOperationException("Invalid request payload.");
        }

        var validationResult = await validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            throw new AppValidationException(validationResult.Errors);
        }

        return await next(context);
    }
}