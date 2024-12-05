namespace SaasLMS.Shared.Models.Payment;

public class CoursePrice
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    
    public decimal BasePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Discount details
    public bool HasDiscount { get; set; }
    public float DiscountPercentage { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    
    // Tax settings
    public bool IsTaxable { get; set; }
    public float TaxPercentage { get; set; }
    
    // Access settings
    public bool IsSubscriptionBased { get; set; }
    public int? AccessDuration { get; set; } // in days
    
    // Status
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}