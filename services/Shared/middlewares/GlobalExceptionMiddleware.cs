using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            SetResponseObjectFields(context, ex);
            var response = new ProblemDetails
            {
                Title = ExceptionsTitles.InvalidOperationError.ToString(),
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Type = ex.GetType().Name
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
        catch (ResourceNotFoundException ex)
        {
            SetResponseObjectFields(context, ex);
            var response = new ProblemDetails
            {
                Title = ExceptionsTitles.NotFoundError.ToString(),
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Type = ex.GetType().Name
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
        catch (AppValidationException ex)
        {
            SetResponseObjectFields(context, ex);
            var response = new ProblemDetails
            {
                Title = ExceptionsTitles.ValidationError.ToString(),
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Type = ex.GetType().Name
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
        catch (ConflictException ex)
        {
            SetResponseObjectFields(context, ex);
            var response = new ProblemDetails
            {
                Title = ExceptionsTitles.ConflictError.ToString(),
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Type = ex.GetType().Name
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GlobalExceptionMiddleware] Unknown exception has occured");

            SetResponseObjectFields(context, ex);

            var response = new ProblemDetails
            {
                Title = ExceptionsTitles.InternalServerError.ToString(),
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Type = ex.GetType().Name
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private static int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            AppValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
    private static void SetResponseObjectFields(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = GetStatusCode(ex);
    }

    public enum ExceptionsTitles
    {
        NotFoundError,
        ValidationError,
        ConflictError,
        UnauthorizedError,
        InternalServerError,
        InvalidOperationError
    }
}