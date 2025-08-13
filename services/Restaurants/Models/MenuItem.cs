using System.ComponentModel.DataAnnotations;
using Restaurants.Utils;

namespace Restaurants.Models;
public class MenuItem: ITenantModel
{
    [Key]
    public int Id { get; set; }
    public Guid RestaurantId { get; set; }

    [MaxLength(Constants.MaxItemNameLength)]
    public string Name { get; set; } = default!;
    public string NormalizedName { get; set; } = default!;

    [MaxLength(Constants.MaxItemDescriptionLength)]
    public string Description { get; set; } = default!;

    [Range(Constants.MinItemPriceDouble, Constants.MaxItemPriceDouble)]
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    public string ImageUrl { get; set; } = default!;
    public string ImagePublicId { get; set; } = default!;
    public ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
}