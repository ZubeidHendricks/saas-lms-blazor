namespace SaasLMS.Shared.Models.CourseStructure;

public class Section
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsLocked { get; set; }
    public int UnlockDays { get; set; } // For drip content
    
    // Relationships
    public virtual List<Lesson> Lessons { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}