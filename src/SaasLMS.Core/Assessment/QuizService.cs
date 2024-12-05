namespace SaasLMS.Core.Assessment;

public class QuizService : IQuizService
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<QuizService> _logger;

    public QuizService(IDbContext dbContext, ILogger<QuizService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Quiz> GetQuizAsync(Guid quizId)
    {
        return await _dbContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == quizId);
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        quiz.Id = Guid.NewGuid();
        _dbContext.Quizzes.Add(quiz);
        await _dbContext.SaveChangesAsync();
        return quiz;
    }

    public async Task UpdateQuizAsync(Quiz quiz)
    {
        _dbContext.Quizzes.Update(quiz);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<QuizAttempt> StartQuizAttemptAsync(Guid quizId, string studentId)
    {
        var quiz = await GetQuizAsync(quizId);
        if (quiz == null)
            throw new NotFoundException("Quiz not found");

        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            StudentId = studentId,
            StartedAt = DateTime.UtcNow
        };

        if (quiz.RandomizeQuestions)
        {
            quiz.Questions = quiz.Questions.OrderBy(q => Guid.NewGuid()).ToList();
        }

        _dbContext.QuizAttempts.Add(attempt);
        await _dbContext.SaveChangesAsync();

        return attempt;
    }

    public async Task<QuizAttempt> SubmitQuizAttemptAsync(Guid attemptId, List<Answer> answers)
    {
        var attempt = await _dbContext.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        if (attempt == null)
            throw new NotFoundException("Quiz attempt not found");

        var quiz = await GetQuizAsync(attempt.QuizId);

        attempt.CompletedAt = DateTime.UtcNow;
        attempt.TimeTaken = attempt.CompletedAt.Value - attempt.StartedAt;
        attempt.Answers = answers;

        // Calculate score
        int totalPoints = quiz.Questions.Sum(q => q.Points);
        int earnedPoints = 0;

        foreach (var answer in answers)
        {
            var question = quiz.Questions.First(q => q.Id == answer.QuestionId);
            bool isCorrect = false;

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                case QuestionType.TrueFalse:
                    isCorrect = question.Options
                        .First(o => o.IsCorrect)
                        .Id.ToString() == answer.Response;
                    break;

                case QuestionType.ShortAnswer:
                    isCorrect = question.CorrectAnswer
                        .Equals(answer.Response, StringComparison.OrdinalIgnoreCase);
                    break;
            }

            if (isCorrect)
            {
                earnedPoints += question.Points;
                answer.IsCorrect = true;
                answer.PointsEarned = question.Points;
            }
        }

        attempt.Score = (int)((earnedPoints / (float)totalPoints) * 100);
        attempt.IsPassed = attempt.Score >= quiz.PassingScore;

        await _dbContext.SaveChangesAsync();
        return attempt;
    }

    public async Task<List<QuizAttempt>> GetStudentAttemptsAsync(Guid quizId, string studentId)
    {
        return await _dbContext.QuizAttempts
            .Where(a => a.QuizId == quizId && a.StudentId == studentId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync();
    }

    public async Task<bool> CanAttemptQuizAsync(Guid quizId, string studentId)
    {
        var quiz = await GetQuizAsync(quizId);
        if (!quiz.IsActive || (quiz.DueDate.HasValue && quiz.DueDate < DateTime.UtcNow))
            return false;

        var attempts = await GetStudentAttemptsAsync(quizId, studentId);
        return attempts.Count < quiz.MaxAttempts;
    }
}