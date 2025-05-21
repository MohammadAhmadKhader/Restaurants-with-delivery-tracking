
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth");

        group.MapGet("/login", async () =>
        {
            var result = new Dictionary<string, object>
            {
                { "message", "hello world" }
            };

            return result;
        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}