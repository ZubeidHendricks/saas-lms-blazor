namespace SaasLMS.Shared.Models;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public float Progress { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProgressRecord> ProgressRecords { get; set; } = new();
}