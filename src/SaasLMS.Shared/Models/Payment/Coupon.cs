namespace SaasLMS.Shared.Models.Payment;

public class Coupon
{
    public Guid Id { get; set; }
    public Guid? CourseId { get; set; } // Null means applicable to all courses
    
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Discount details
    public DiscountType Type { get; set; }
    public decimal Amount { get; set; } // Percentage or fixed amount
    public decimal? MaximumDiscount { get; set; }
    
    // Usage limits
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsOneTimeUse { get; set; }
    
    // Validity
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}