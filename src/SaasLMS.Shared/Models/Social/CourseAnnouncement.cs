namespace SaasLMS.Shared.Models.Social;

public class CourseAnnouncement
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsPinned { get; set; }
    
    // Email notification
    public bool SendEmail { get; set; }
    public bool EmailSent { get; set; }
    public DateTime? EmailSentAt { get; set; }
    
    // Metrics
    public int ViewCount { get; set; }
    public List<Guid> ViewedByUsers { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}