using Reviews.Contracts.Dtos.MenuItemReview;

namespace Reviews.Endpoints;
public static class MenuItemReviewEndpoints
{
    public static void MapMenuItemReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews/menu-items");

        group.MapGet("/{id}", async (int id) =>
        {

        });

        group.MapPost("/", async (MenuItemReviewAddDto dto) =>
        {

        });
    }
}