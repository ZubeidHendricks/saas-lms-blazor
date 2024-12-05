namespace SaasLMS.Core.Progress;

public class StudentProgress
{
    public Guid Id { get; set; }
    public string StudentId { get; set; }
    public Guid CourseId { get; set; }
    public List<ModuleProgress> ModuleProgress { get; set; } = new();
    public float OverallProgress { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ModuleProgress
{
    public Guid ModuleId { get; set; }
    public List<ContentProgress> ContentProgress { get; set; } = new();
    public List<AssessmentAttempt> AssessmentAttempts { get; set; } = new();
    public float CompletionPercentage { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ContentProgress
{
    public Guid ContentId { get; set; }
    public bool IsCompleted { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public DateTime? LastAccessedAt { get; set; }
}

public class AssessmentAttempt
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public List<QuestionResponse> Responses { get; set; } = new();
}