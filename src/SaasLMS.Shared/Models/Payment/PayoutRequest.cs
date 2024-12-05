namespace SaasLMS.Shared.Models.Payment;

public class PayoutRequest
{
    public Guid Id { get; set; }
    public Guid InstructorId { get; set; }
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PayoutStatus Status { get; set; }
    
    // Payment details
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentDetails { get; set; } = string.Empty; // JSON of payment info
    
    // Processing details
    public string? ProcessingNotes { get; set; }
    public string? RejectionReason { get; set; }
    public string? TransactionId { get; set; }
    
    // Related earnings
    public List<InstructorEarning> Earnings { get; set; } = new();
    
    // Timestamps
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}