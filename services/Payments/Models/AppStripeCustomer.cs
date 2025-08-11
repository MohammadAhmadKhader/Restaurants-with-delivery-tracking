using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payments.Models;

[Table("StripeCustomers")]
public class AppStripeCustomer
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(64)]
    [Required]
    public string StripeCustomerId { get; set; } = default!;
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}