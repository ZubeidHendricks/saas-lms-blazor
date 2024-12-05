namespace SaasLMS.Shared.Models.Enrollment;

public class AssignmentSubmission
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AttachmentUrl { get; set; } = string.Empty;
    
    // Review
    public string? ReviewNotes { get; set; }
    public float? Score { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public AssignmentStatus Status { get; set; }
    
    // Timestamps
    public DateTime SubmittedAt { get; set; }
    public DateTime? DueDate { get; set; }
}