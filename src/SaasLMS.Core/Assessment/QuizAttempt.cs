namespace SaasLMS.Core.Assessment;

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string StudentId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public TimeSpan TimeTaken { get; set; }
}

public class Answer
{
    public Guid QuestionId { get; set; }
    public string Response { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public DateTime AnsweredAt { get; set; }
}