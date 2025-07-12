using Microsoft.AspNetCore.Mvc;

namespace Shared.Exceptions;
public class HttpProblemDetailsException(ProblemDetails details) : Exception(details?.Detail ?? "Internal Server Error.")
{
    public ProblemDetails ProblemDetails { get; } = details;
}