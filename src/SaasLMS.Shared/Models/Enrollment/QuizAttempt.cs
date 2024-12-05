namespace SaasLMS.Shared.Models.Enrollment;

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public float Score { get; set; }
    public int Duration { get; set; } // in seconds
    public int AttemptNumber { get; set; }
    public string Answers { get; set; } = string.Empty; // JSON data of answers
    
    // Results
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public bool IsPassed { get; set; }
    
    // Timestamps
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}