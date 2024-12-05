namespace SaasLMS.Core.Assessments;

public class Question
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public List<QuestionOption> Options { get; set; } = new();
    public string CorrectAnswer { get; set; }
    public string Explanation { get; set; }
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    ShortAnswer,
    Essay,
    FileUpload
}

public class QuestionOption
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}