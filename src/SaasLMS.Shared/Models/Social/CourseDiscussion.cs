namespace SaasLMS.Shared.Models.Social;

public class CourseDiscussion
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsAnnouncement { get; set; }
    public bool IsPinned { get; set; }
    
    // Discussion metrics
    public int Likes { get; set; }
    public int Replies { get; set; }
    public int Views { get; set; }
    
    // Status
    public DiscussionStatus Status { get; set; }
    public bool IsResolved { get; set; }
    
    // Relationships
    public virtual List<CourseDiscussion> Responses { get; set; } = new();
    public virtual List<DiscussionReaction> Reactions { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
}