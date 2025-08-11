using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Payments.Models;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; } = default!;
    public string PaymentIntentId { get; set; } = default!;
    public string ChargeId { get; set; } = default!;
    public string CardLast4 { get; set; } = default!;
    public string CardBrand { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public string ReceiptUrl { get; set; } = default!;
    public string PaymentMethod { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public AppStripeCustomer Customer { get; set; } = default!;
}