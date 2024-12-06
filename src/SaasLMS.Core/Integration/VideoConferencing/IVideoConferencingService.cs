namespace SaasLMS.Core.Integration.VideoConferencing;

public interface IVideoConferencingService
{
    Task<Meeting> CreateMeetingAsync(MeetingRequest request);
    Task<Meeting> GetMeetingDetailsAsync(string meetingId);
    Task<bool> UpdateMeetingAsync(string meetingId, MeetingRequest request);
    Task<bool> DeleteMeetingAsync(string meetingId);
    Task<List<MeetingParticipant>> GetParticipantsAsync(string meetingId);
    Task<MeetingRecording> GetRecordingAsync(string meetingId);
}

public class MeetingRequest
{
    public string Topic { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public string TimeZone { get; set; }
    public string HostEmail { get; set; }
    public bool EnableWaitingRoom { get; set; }
    public bool EnableRecording { get; set; }
    public MeetingPlatform Platform { get; set; }
    public MeetingSettings Settings { get; set; } = new();
}

public class Meeting
{
    public string Id { get; set; }
    public string Topic { get; set; }
    public string JoinUrl { get; set; }
    public string StartUrl { get; set; }
    public string Password { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public MeetingStatus Status { get; set; }
    public MeetingPlatform Platform { get; set; }
}

public class MeetingParticipant
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime JoinTime { get; set; }
    public DateTime? LeaveTime { get; set; }
    public TimeSpan Duration => LeaveTime?.Subtract(JoinTime) ?? TimeSpan.Zero;
}

public class MeetingRecording
{
    public string Id { get; set; }
    public string MeetingId { get; set; }
    public DateTime RecordingStart { get; set; }
    public DateTime RecordingEnd { get; set; }
    public long FileSize { get; set; }
    public string DownloadUrl { get; set; }
    public string PlayUrl { get; set; }
}

public class MeetingSettings
{
    public bool MuteUponEntry { get; set; }
    public bool EnableChat { get; set; }
    public bool EnablePolling { get; set; }
    public bool EnableBreakoutRooms { get; set; }
    public bool RequireAuthentication { get; set; }
}

public enum MeetingStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public enum MeetingPlatform
{
    Zoom,
    Teams
}