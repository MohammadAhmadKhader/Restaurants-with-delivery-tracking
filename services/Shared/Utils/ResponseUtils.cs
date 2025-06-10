using System.Net;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions;

namespace Shared.Utils;

public class ResponseUtils
{
    public static IResult Forbidden(string detail = "Forbidden")
    {
        return Results.Problem(statusCode: (int)HttpStatusCode.Forbidden, detail: detail);
    }

    public static IResult Unauthorized(string detail = "Unauthorized")
    {
        return Results.Problem(statusCode: (int) HttpStatusCode.Unauthorized, detail: detail);
    }

    public static IResult NotFound(string resourceName)
    {
        var message = ResourceNotFoundException.ErrorMessageFormmatter(resourceName);
        return Results.Problem(statusCode: (int) HttpStatusCode.NotFound, detail: message);
    }

    public static IResult NotFound(string resourceName, object id)
    {
        var message = ResourceNotFoundException.ErrorMessageWithIdFormmatter(resourceName, id);
        return Results.Problem(statusCode: (int) HttpStatusCode.NotFound, detail: message);
    }

    public static IResult Error(string detail, HttpStatusCode statusCode)
    {
        return Results.Problem(statusCode: (int) statusCode, detail: detail);
    }

    public static IResult Error(string detail, int statusCode = 400)
    {
        return Results.Problem(statusCode: statusCode, detail: detail);
    }
    
    public static IResult InternalError()
    {
        return Results.Problem(statusCode: (int) HttpStatusCode.InternalServerError, detail: "Internal Server Error");
    }
}