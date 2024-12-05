namespace SaasLMS.Core.Assessment;

public class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeLimit { get; set; }  // in minutes
    public int PassingScore { get; set; } // percentage
    public int MaxAttempts { get; set; }
    public bool RandomizeQuestions { get; set; }
    public bool ShowAnswersAfterSubmission { get; set; }
    public List<Question> Questions { get; set; } = new();
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public List<Option> Options { get; set; } = new();
    public string CorrectAnswer { get; set; }  // For short answer questions
    public string Explanation { get; set; }
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    ShortAnswer,
    Essay
}

public class Option
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}