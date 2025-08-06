using Reviews.Contracts.Dtos.MenuItemReview;
using Reviews.Models;

namespace Reviews.Services.IServices;

public interface IMenuItemReviewsService
{
    Task<MenuItemReview> AddReview(int itemId, MenuItemReviewAddDto dto);
    Task<MenuItemReviewsStatus> GetItemReviewsStatus(int itemId);
}