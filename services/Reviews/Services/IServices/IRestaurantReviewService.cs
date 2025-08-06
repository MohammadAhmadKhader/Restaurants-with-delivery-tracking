using Reviews.Contracts.Dtos.RestaurantReview;
using Reviews.Models;

namespace Reviews.Services.IServices;
public interface IRestaurantReviewService
{
    Task<RestaurantReview> AddReview(Guid restaurantId, RestaurantReviewAddDto dto);
    Task<RestaurantReviewsStatus> GetReviewsStatus(Guid restaurantId);
}