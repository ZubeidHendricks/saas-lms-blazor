namespace SaasLMS.Core.LiveSessions.Services;

public interface ILiveSessionService
{
    Task<ModuleSession> CreateSessionAsync(ModuleSession session);
    Task<ModuleSession> GetSessionAsync(Guid sessionId);
    Task<List<ModuleSession>> GetActiveSessionsForUserAsync(string userId);
    Task<bool> RecordParticipantJoinAsync(Guid sessionId, string userId);
    Task<bool> RecordParticipantLeaveAsync(Guid sessionId, string userId);
    Task<SessionPoll> CreatePollAsync(SessionPoll poll);
    Task<SessionPoll> EndPollAsync(Guid pollId);
    Task<PollResponse> RecordPollResponseAsync(PollResponse response);
    Task<SessionQuiz> CreateQuizAsync(SessionQuiz quiz);
    Task<SessionQuiz> EndQuizAsync(Guid quizId);
    Task<QuizAttempt> RecordQuizAttemptAsync(QuizAttempt attempt);
    Task<bool> RecordAttentionStatusAsync(Guid sessionId, string userId, AttentionStatus status);
}

public class LiveSessionService : ILiveSessionService
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<LiveSessionService> _logger;

    public LiveSessionService(
        IDbContext dbContext,
        ILogger<LiveSessionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ModuleSession> CreateSessionAsync(ModuleSession session)
    {
        _dbContext.ModuleSessions.Add(session);
        await _dbContext.SaveChangesAsync();
        return session;
    }

    public async Task<ModuleSession> GetSessionAsync(Guid sessionId)
    {
        return await _dbContext.ModuleSessions
            .Include(s => s.Module)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task<List<ModuleSession>> GetActiveSessionsForUserAsync(string userId)
    {
        return await _dbContext.ModuleSessionParticipants
            .Where(p => p.UserId == userId && !p.LeftAt.HasValue)
            .Select(p => p.Session)
            .ToListAsync();
    }

    public async Task<bool> RecordParticipantJoinAsync(Guid sessionId, string userId)
    {
        try
        {
            var participant = new ModuleSessionParticipant
            {
                SessionId = sessionId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _dbContext.ModuleSessionParticipants.Add(participant);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording participant join");
            return false;
        }
    }

    public async Task<bool> RecordParticipantLeaveAsync(Guid sessionId, string userId)
    {
        try
        {
            var participant = await _dbContext.ModuleSessionParticipants
                .FirstOrDefaultAsync(p => 
                    p.SessionId == sessionId && 
                    p.UserId == userId && 
                    !p.LeftAt.HasValue);

            if (participant != null)
            {
                participant.LeftAt = DateTime.UtcNow;
                participant.Duration = participant.LeftAt.Value - participant.JoinedAt;
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording participant leave");
            return false;
        }
    }

    public async Task<SessionPoll> CreatePollAsync(SessionPoll poll)
    {
        poll.CreatedAt = DateTime.UtcNow;
        poll.Status = PollStatus.Active;

        _dbContext.SessionPolls.Add(poll);
        await _dbContext.SaveChangesAsync();
        return poll;
    }

    public async Task<SessionPoll> EndPollAsync(Guid pollId)
    {
        var poll = await _dbContext.SessionPolls
            .Include(p => p.Responses)
            .FirstOrDefaultAsync(p => p.Id == pollId);

        if (poll != null)
        {
            poll.Status = PollStatus.Ended;
            poll.EndedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return poll;
    }

    public async Task<PollResponse> RecordPollResponseAsync(PollResponse response)
    {
        response.SubmittedAt = DateTime.UtcNow;
        _dbContext.PollResponses.Add(response);
        await _dbContext.SaveChangesAsync();
        return response;
    }

    public async Task<SessionQuiz> CreateQuizAsync(SessionQuiz quiz)
    {
        quiz.CreatedAt = DateTime.UtcNow;
        quiz.Status = QuizStatus.Active;

        _dbContext.SessionQuizzes.Add(quiz);
        await _dbContext.SaveChangesAsync();
        return quiz;
    }

    public async Task<SessionQuiz> EndQuizAsync(Guid quizId)
    {
        var quiz = await _dbContext.SessionQuizzes
            .Include(q => q.Attempts)
            .FirstOrDefaultAsync(q => q.Id == quizId);

        if (quiz != null)
        {
            quiz.Status = QuizStatus.Ended;
            quiz.EndedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return quiz;
    }

    public async Task<QuizAttempt> RecordQuizAttemptAsync(QuizAttempt attempt)
    {
        attempt.CompletedAt = DateTime.UtcNow;

        var quiz = await _dbContext.SessionQuizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == attempt.QuizId);

        if (quiz != null)
        {
            var totalPoints = quiz.Questions.Sum(q => q.Points);
            var earnedPoints = 0;

            foreach (var answer in attempt.Answers)
            {
                var question = quiz.Questions
                    .First(q => q.Id == answer.QuestionId);

                answer.IsCorrect = question.CorrectAnswer == answer.Response;
                if (answer.IsCorrect)
                {
                    answer.PointsEarned = question.Points;
                    earnedPoints += question.Points;
                }
            }

            attempt.Score = (int)((earnedPoints / (float)totalPoints) * 100);
            attempt.IsPassed = attempt.Score >= quiz.PassingScore;
        }

        _dbContext.QuizAttempts.Add(attempt);
        await _dbContext.SaveChangesAsync();
        return attempt;
    }

    public async Task<bool> RecordAttentionStatusAsync(
        Guid sessionId,
        string userId,
        AttentionStatus status)
    {
        try
        {
            var metrics = new AttentionMetrics
            {
                SessionId = sessionId,
                UserId = userId,
                Status = status,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.AttentionMetrics.Add(metrics);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording attention status");
            return false;
        }
    }
}