namespace SaasLMS.Shared.Models.Enrollment;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public decimal PricePaid { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public EnrollmentStatus Status { get; set; }
    
    // Progress tracking
    public float Progress { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Certificate
    public bool IsCertificateIssued { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime? CertificateIssuedAt { get; set; }
    
    // Timestamps
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; } // For subscription-based courses
    
    // Relationships
    public virtual List<LessonCompletion> LessonCompletions { get; set; } = new();
    public virtual List<QuizAttempt> QuizAttempts { get; set; } = new();
    public virtual List<AssignmentSubmission> AssignmentSubmissions { get; set; } = new();
}