namespace SaasLMS.Shared.Models.Payment;

public class InstructorEarning
{
    public Guid Id { get; set; }
    public Guid InstructorId { get; set; }
    public Guid CourseId { get; set; }
    public Guid TransactionId { get; set; }
    
    // Earnings breakdown
    public decimal CoursePrice { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal EarnedAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal TaxWithheld { get; set; }
    
    // Payment status
    public bool IsPaid { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime? PaidAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
}