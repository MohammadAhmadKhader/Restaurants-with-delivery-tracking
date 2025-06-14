using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Reviews.Models;

[Index(nameof(OrderId), IsUnique = true)]
public class MenuItemReview
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid CustomerId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
