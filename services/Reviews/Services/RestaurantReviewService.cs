using Reviews.Contracts.Dtos.RestaurantReview;
using Reviews.Models;
using Reviews.Services.IServices;

namespace Reviews.Services;

public class RestaurantReviewService : IRestaurantReviewService
{
    public Task<RestaurantReview> AddReview(Guid restaurantId, RestaurantReviewAddDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<RestaurantReviewsStatus> GetReviewsStatus(Guid restaurantId)
    {
        throw new NotImplementedException();
    }
}