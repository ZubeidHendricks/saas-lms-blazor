namespace SaasLMS.Core.Assessment;

public interface IQuizService
{
    Task<Quiz> GetQuizAsync(Guid quizId);
    Task<Quiz> CreateQuizAsync(Quiz quiz);
    Task UpdateQuizAsync(Quiz quiz);
    Task<QuizAttempt> StartQuizAttemptAsync(Guid quizId, string studentId);
    Task<QuizAttempt> SubmitQuizAttemptAsync(Guid attemptId, List<Answer> answers);
    Task<List<QuizAttempt>> GetStudentAttemptsAsync(Guid quizId, string studentId);
    Task<bool> CanAttemptQuizAsync(Guid quizId, string studentId);
}