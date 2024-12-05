namespace SaasLMS.Shared.Models.Enrollment;

public class LessonCompletion
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public CompletionStatus Status { get; set; }
    public float Progress { get; set; }
    public int TimeSpent { get; set; } // in seconds
    
    // Video specific tracking
    public float VideoProgress { get; set; }
    public int LastVideoPosition { get; set; } // in seconds
    
    // Quiz specific tracking
    public float? QuizScore { get; set; }
    public int QuizAttempts { get; set; }
    
    // Assignment specific tracking
    public string? AssignmentStatus { get; set; }
    public float? AssignmentScore { get; set; }
    
    // Timestamps
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
}