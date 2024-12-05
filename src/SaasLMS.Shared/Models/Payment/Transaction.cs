namespace SaasLMS.Shared.Models.Payment;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    
    // Amount details
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal ProcessingFee { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Payment details
    public PaymentStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentMethodId { get; set; }
    
    // Item details
    public TransactionType Type { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? BundleId { get; set; }
    public Guid? SubscriptionId { get; set; }
    
    // Refund details
    public bool IsRefunded { get; set; }
    public decimal? RefundedAmount { get; set; }
    public string? RefundReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}