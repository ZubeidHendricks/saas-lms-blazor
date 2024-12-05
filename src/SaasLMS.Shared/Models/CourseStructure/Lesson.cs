namespace SaasLMS.Shared.Models.CourseStructure;

public class Lesson
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public int OrderIndex { get; set; }
    public int Duration { get; set; } // in minutes
    public bool IsPreviewable { get; set; }
    
    // Content based on type
    public string VideoUrl { get; set; } = string.Empty;
    public string VideoProvider { get; set; } = string.Empty; // YouTube, Vimeo, etc.
    public string DocumentUrl { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string QuizData { get; set; } = string.Empty; // JSON data for quiz
    public string AssignmentData { get; set; } = string.Empty; // JSON data for assignment
    
    // Live class specific
    public string MeetingUrl { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    // Completion tracking
    public bool RequiresCompletion { get; set; }
    public int MinimumScore { get; set; } // For quizzes
    
    // Relationships
    public virtual List<LessonAttachment> Attachments { get; set; } = new();
    public virtual List<LessonCompletion> Completions { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}