namespace SaasLMS.Shared.Models;

public class ProgressRecord
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid ContentId { get; set; }
    public ProgressStatus Status { get; set; }
    public float Progress { get; set; }
    public int TimeSpent { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}