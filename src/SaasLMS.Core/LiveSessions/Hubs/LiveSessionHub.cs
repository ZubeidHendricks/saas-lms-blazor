namespace SaasLMS.Core.LiveSessions.Hubs;

public class LiveSessionHub : Hub
{
    private readonly ILiveSessionService _sessionService;
    private readonly ILogger<LiveSessionHub> _logger;

    public LiveSessionHub(
        ILiveSessionService sessionService,
        ILogger<LiveSessionHub> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await _sessionService.RecordParticipantJoinAsync(
            Guid.Parse(sessionId),
            Context.UserIdentifier);

        await Clients.Group(sessionId).SendAsync(
            "ParticipantJoined",
            Context.UserIdentifier);
    }

    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await _sessionService.RecordParticipantLeaveAsync(
            Guid.Parse(sessionId),
            Context.UserIdentifier);

        await Clients.Group(sessionId).SendAsync(
            "ParticipantLeft",
            Context.UserIdentifier);
    }

    public async Task LaunchPoll(string sessionId, SessionPoll poll)
    {
        var savedPoll = await _sessionService.CreatePollAsync(poll);
        await Clients.Group(sessionId).SendAsync("PollLaunched", savedPoll);
    }

    public async Task EndPoll(string sessionId, Guid pollId)
    {
        var poll = await _sessionService.EndPollAsync(pollId);
        await Clients.Group(sessionId).SendAsync("PollEnded", poll);
    }

    public async Task SubmitPollResponse(string sessionId, PollResponse response)
    {
        var savedResponse = await _sessionService.RecordPollResponseAsync(response);
        await Clients.Group(sessionId).SendAsync("PollResponseReceived", savedResponse);
    }

    public async Task LaunchQuiz(string sessionId, SessionQuiz quiz)
    {
        var savedQuiz = await _sessionService.CreateQuizAsync(quiz);
        await Clients.Group(sessionId).SendAsync("QuizLaunched", savedQuiz);
    }

    public async Task EndQuiz(string sessionId, Guid quizId)
    {
        var quiz = await _sessionService.EndQuizAsync(quizId);
        await Clients.Group(sessionId).SendAsync("QuizEnded", quiz);
    }

    public async Task SubmitQuizAttempt(string sessionId, QuizAttempt attempt)
    {
        var savedAttempt = await _sessionService.RecordQuizAttemptAsync(attempt);
        await Clients.Group(sessionId).SendAsync("QuizAttemptReceived", savedAttempt);
    }

    public async Task UpdateAttentionStatus(
        string sessionId,
        string userId,
        AttentionStatus status)
    {
        await _sessionService.RecordAttentionStatusAsync(
            Guid.Parse(sessionId),
            userId,
            status);

        // Only send to hosts
        await Clients.Group($"{sessionId}-hosts").SendAsync(
            "AttentionStatusUpdated",
            userId,
            status);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var activeSessions = await _sessionService.GetActiveSessionsForUserAsync(
            Context.UserIdentifier);

        foreach (var session in activeSessions)
        {
            await LeaveSession(session.Id.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }
}

public enum AttentionStatus
{
    Focused,
    Unfocused,
    Away
}