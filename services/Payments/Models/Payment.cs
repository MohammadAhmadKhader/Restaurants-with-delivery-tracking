using System.ComponentModel.DataAnnotations;

namespace Payments.Models;
public enum PaymentStatus { PENDING, PAID, FAILED }
public enum PaymentMethod { CREDIT_CARD, DEBIT_CARD, UPI, CASH_ON_DELIVERY }
public class Payment
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}