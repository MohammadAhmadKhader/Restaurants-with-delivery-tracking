using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Reviews.Models;

[Index(nameof(CustomerId), nameof(RestaurantId), IsUnique = true)]
public class RestaurantReview
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
