using Reviews.Contracts.Dtos.MenuItemReview;
using Reviews.Models;
using Reviews.Services.IServices;

namespace Reviews.Services;

public class MenuItemReviewsService : IMenuItemReviewsService
{
    public Task<MenuItemReview> AddReview(int itemId, MenuItemReviewAddDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<MenuItemReviewsStatus> GetItemReviewsStatus(int itemId)
    {
        throw new NotImplementedException();
    }
}