namespace Restaurants.Utils;

class Constants
{
    // restaurant
    public const int MaxRestaurantNameLength = 36;
    public const int MinRestaurantNameLength = 2;
    public const int MaxRestaurantDescriptionLength = 256;
    public const int MinRestaurantDescriptionLength = 4;

    // menu-item
    public const int MaxItemNameLength = 36;
    public const int MinItemNameLength = 2;
    public const int MaxItemDescriptionLength = 256;
    public const int MinItemDescriptionLength = 4;

    public const decimal MaxItemPrice = 1000m;
    public const decimal MinItemPrice = 0.5m;
    public const double MinItemPriceDouble = 0.5;
    public const double MaxItemPriceDouble = 1000.0;

    // menu
    public const int MaxMenuNameLength = 36;
    public const int MinMenuNameLength = 2;
    public const int MaxMenuCategoryLength = 36;
    public const int MinMenuCategoryLength = 2;

    public const int MaxEmailLength = 64;
}