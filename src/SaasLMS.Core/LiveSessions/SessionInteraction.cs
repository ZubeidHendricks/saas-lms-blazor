namespace SaasLMS.Core.LiveSessions;

public class SessionPoll
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Question { get; set; }
    public List<PollOption> Options { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsAnonymous { get; set; }
    public PollStatus Status { get; set; }
    public virtual ModuleSession Session { get; set; }
    public virtual List<PollResponse> Responses { get; set; } = new();
}

public class PollOption
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public string Text { get; set; }
    public int Order { get; set; }
    public virtual SessionPoll Poll { get; set; }
}

public class PollResponse
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid OptionId { get; set; }
    public string UserId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public virtual SessionPoll Poll { get; set; }
    public virtual PollOption Option { get; set; }
    public virtual ApplicationUser User { get; set; }
}

public enum PollStatus
{
    Active,
    Ended,
    Archived
}

public class SessionQuiz
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Title { get; set; }
    public List<QuizQuestion> Questions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public bool ShowResults { get; set; }
    public QuizStatus Status { get; set; }
    public virtual ModuleSession Session { get; set; }
    public virtual List<QuizAttempt> Attempts { get; set; } = new();
}

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public List<QuizOption> Options { get; set; } = new();
    public string CorrectAnswer { get; set; }
    public int Points { get; set; }
    public int Order { get; set; }
    public virtual SessionQuiz Quiz { get; set; }
}

public class QuizOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
    public virtual QuizQuestion Question { get; set; }
}

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public List<QuizAnswer> Answers { get; set; } = new();
    public virtual SessionQuiz Quiz { get; set; }
    public virtual ApplicationUser User { get; set; }
}

public class QuizAnswer
{
    public Guid Id { get; set; }
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string Response { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public DateTime SubmittedAt { get; set; }
    public virtual QuizAttempt Attempt { get; set; }
    public virtual QuizQuestion Question { get; set; }
}