namespace SaasLMS.Shared.Models.Enums;

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}

public enum TransactionType
{
    CourseEnrollment,
    BundlePurchase,
    Subscription,
    Other
}

public enum DiscountType
{
    Percentage,
    FixedAmount
}

public enum PayoutStatus
{
    Requested,
    UnderReview,
    Approved,
    Processing,
    Completed,
    Rejected
}