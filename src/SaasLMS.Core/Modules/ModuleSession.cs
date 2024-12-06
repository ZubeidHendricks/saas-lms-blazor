namespace SaasLMS.Core.Modules;

public class ModuleSession
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string MeetingId { get; set; }
    public MeetingPlatform Platform { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedById { get; set; }
    public virtual Module Module { get; set; }
    public virtual ApplicationUser CreatedBy { get; set; }
}

public class ModuleSessionParticipant
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public virtual ModuleSession Session { get; set; }
    public virtual ApplicationUser User { get; set; }
}

public class ModuleSessionRecording
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string RecordingUrl { get; set; }
    public string DownloadUrl { get; set; }
    public long FileSize { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ModuleSession Session { get; set; }
}

public class ModuleSessionTranscript
{
    public Guid Id { get; set; }
    public Guid RecordingId { get; set; }
    public string Content { get; set; }
    public List<TranscriptSegment> Segments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ModuleSessionRecording Recording { get; set; }
}

public class TranscriptSegment
{
    public Guid Id { get; set; }
    public Guid TranscriptId { get; set; }
    public string Text { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string SpeakerId { get; set; }
    public virtual ModuleSessionTranscript Transcript { get; set; }
}