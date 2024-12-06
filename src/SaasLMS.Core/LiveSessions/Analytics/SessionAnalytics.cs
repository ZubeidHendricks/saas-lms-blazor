namespace SaasLMS.Core.LiveSessions.Analytics;

public class SessionAnalytics
{
    public Guid SessionId { get; set; }
    public int TotalParticipants { get; set; }
    public int PeakParticipants { get; set; }
    public TimeSpan AverageAttendanceTime { get; set; }
    public double EngagementScore { get; set; }
    public List<ParticipantMetrics> ParticipantMetrics { get; set; } = new();
    public List<InteractionMetrics> InteractionMetrics { get; set; } = new();
    public List<AttentionMetrics> AttentionMetrics { get; set; } = new();
}

public class ParticipantMetrics
{
    public string UserId { get; set; }
    public DateTime JoinTime { get; set; }
    public DateTime? LeaveTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public int PollResponses { get; set; }
    public int QuizAttempts { get; set; }
    public int QuestionsAsked { get; set; }
    public double AttentionScore { get; set; }
}

public class InteractionMetrics
{
    public DateTime Timestamp { get; set; }
    public InteractionType Type { get; set; }
    public int ParticipantCount { get; set; }
    public double ResponseRate { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
}

public class AttentionMetrics
{
    public DateTime Timestamp { get; set; }
    public int ActiveParticipants { get; set; }
    public int TabFocused { get; set; }
    public int WindowMinimized { get; set; }
    public double AverageAttentionScore { get; set; }
}

public enum InteractionType
{
    Poll,
    Quiz,
    Question,
    Reaction,
    Chat
}