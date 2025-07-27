using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace Shared.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            var validationErrors = ex.Errors.GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            ).ToList();

            var result = Results.ValidationProblem(
                validationErrors,
                title: ExceptionsTitles.ValidationError.ToString(),
                statusCode: StatusCodes.Status400BadRequest
            );

            await result.ExecuteAsync(context);
        }
        catch (HttpProblemDetailsException ex)
        {
            var problem = Results.Problem(ex.ProblemDetails);
            await problem.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GlobalExceptionMiddleware] Exception occurred");

            if (ex is BadHttpRequestException badReq && badReq.Message.StartsWith(BindingErrorMessageStart))
            {
                var start = BindingErrorMessageStart.Length;
                var end = badReq.Message.IndexOf("\" from", start);

                var typeAndName = badReq.Message.Substring(start, end - start);
                var parts = typeAndName.Split(' ');
                var paramName = parts[1];

                var statusCode = StatusCodes.Status400BadRequest;
                var problem = Results.Problem(
                    statusCode: statusCode,
                    title: ExceptionsTitles.BindingError.ToString(),
                    detail: string.Format("Invalid {0}.", paramName)
                );

                await problem.ExecuteAsync(context);
            }
            else
            {
                var (statusCode, title) = GetExceptionMetadata(ex);

                var problem = Results.Problem(
                    statusCode: statusCode,
                    title: title,
                    detail: ex.Message
                );

                await problem.ExecuteAsync(context);
            }
        }
    }

    private static (int StatusCode, string Title) GetExceptionMetadata(Exception ex)
    {
        return ex switch
        {
            InvalidOperationException => (StatusCodes.Status400BadRequest, nameof(ExceptionsTitles.InvalidOperationError)),
            AppValidationException => (StatusCodes.Status400BadRequest, nameof(ExceptionsTitles.ValidationError)),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, nameof(ExceptionsTitles.UnauthorizedError)),
            ResourceNotFoundException => (StatusCodes.Status404NotFound, nameof(ExceptionsTitles.NotFoundError)),
            ConflictException => (StatusCodes.Status409Conflict, nameof(ExceptionsTitles.ConflictError)),
            _ => (StatusCodes.Status500InternalServerError, nameof(ExceptionsTitles.InternalServerError))
        };
    }
    public enum ExceptionsTitles
    {
        NotFoundError,
        ValidationError,
        ConflictError,
        BindingError,
        UnauthorizedError,
        InternalServerError,
        InvalidOperationError
    }

    private const string BindingErrorMessageStart = "Failed to bind parameter \"";
}