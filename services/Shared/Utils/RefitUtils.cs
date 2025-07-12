using Refit;
using Shared.Exceptions;
using Serilog;

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Shared.Utils;

public static class RefitUtils
{
    public static async Task<T> TryCall<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (ApiException ex)
        {
            var details = await ex.GetContentAsAsync<ProblemDetails>();
            if (details == null)
            {
                Log.Information("Error body is expected to be a typeof: {ExpectedType} but received {InvalidType}",
                typeof(ProblemDetails), details?.Type ?? "null");
                throw;
            }

            throw new HttpProblemDetailsException(details);
        }
    }
}