namespace SaasLMS.Core.Integration.VideoConferencing;

public class TeamsService : IVideoConferencingService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IOptions<TeamsSettings> _settings;
    private readonly ILogger<TeamsService> _logger;

    public TeamsService(
        GraphServiceClient graphClient,
        IOptions<TeamsSettings> settings,
        ILogger<TeamsService> logger)
    {
        _graphClient = graphClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<Meeting> CreateMeetingAsync(MeetingRequest request)
    {
        try
        {
            var onlineMeeting = new OnlineMeeting
            {
                StartDateTime = request.StartTime,
                EndDateTime = request.StartTime.AddMinutes(request.DurationMinutes),
                Subject = request.Topic,
                LobbyBypassSettings = new LobbyBypassSettings
                {
                    Scope = request.EnableWaitingRoom ? 
                        LobbyBypassScope.Organizer : 
                        LobbyBypassScope.Everyone
                },
                AudioConferencing = new AudioConferencing
                {
                    TollNumber = _settings.Value.AudioConferenceNumber
                },
                ChatInfo = new ChatInfo
                {
                    ThreadId = await CreateTeamsChatThread(request.Topic)
                },
                AllowedPresenters = OnlineMeetingPresenters.EveryoneInCompany
            };

            var createdMeeting = await _graphClient.Users[request.HostEmail]
                .OnlineMeetings
                .Request()
                .AddAsync(onlineMeeting);

            return new Meeting
            {
                Id = createdMeeting.Id,
                Topic = createdMeeting.Subject,
                JoinUrl = createdMeeting.JoinWebUrl,
                StartUrl = createdMeeting.JoinWebUrl,
                StartTime = createdMeeting.StartDateTime.Value,
                DurationMinutes = (int)createdMeeting.EndDateTime.Value
                    .Subtract(createdMeeting.StartDateTime.Value)
                    .TotalMinutes,
                Status = MeetingStatus.Scheduled,
                Platform = MeetingPlatform.Teams
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Teams meeting");
            throw new VideoConferencingException("Failed to create Teams meeting", ex);
        }
    }

    public async Task<Meeting> GetMeetingDetailsAsync(string meetingId)
    {
        try
        {
            var meeting = await _graphClient.Users[_settings.Value.OrganizerId]
                .OnlineMeetings[meetingId]
                .Request()
                .GetAsync();

            return new Meeting
            {
                Id = meeting.Id,
                Topic = meeting.Subject,
                JoinUrl = meeting.JoinWebUrl,
                StartUrl = meeting.JoinWebUrl,
                StartTime = meeting.StartDateTime.Value,
                DurationMinutes = (int)meeting.EndDateTime.Value
                    .Subtract(meeting.StartDateTime.Value)
                    .TotalMinutes,
                Status = GetMeetingStatus(meeting),
                Platform = MeetingPlatform.Teams
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Teams meeting details");
            throw new VideoConferencingException("Failed to get Teams meeting details", ex);
        }
    }

    public async Task<List<MeetingParticipant>> GetParticipantsAsync(string meetingId)
    {
        try
        {
            var attendance = await _graphClient.Users[_settings.Value.OrganizerId]
                .OnlineMeetings[meetingId]
                .Attendance
                .Request()
                .GetAsync();

            return attendance.Select(a => new MeetingParticipant
            {
                Id = a.Id,
                Name = a.Identity.DisplayName,
                Email = a.Identity.Email,
                JoinTime = a.JoinDateTime.Value,
                LeaveTime = a.LeaveDateTime
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Teams meeting participants");
            throw new VideoConferencingException(
                "Failed to get Teams meeting participants", ex);
        }
    }

    public async Task<MeetingRecording> GetRecordingAsync(string meetingId)
    {
        try
        {
            var recording = await _graphClient.Users[_settings.Value.OrganizerId]
                .OnlineMeetings[meetingId]
                .Recording
                .Request()
                .GetAsync();

            if (recording == null)
                return null;

            return new MeetingRecording
            {
                Id = recording.Id,
                MeetingId = meetingId,
                RecordingStart = recording.CreatedDateTime.Value,
                RecordingEnd = recording.CreatedDateTime.Value.AddSeconds(
                    recording.Length ?? 0),
                FileSize = recording.Size ?? 0,
                DownloadUrl = recording.DownloadUrl,
                PlayUrl = recording.PlayUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Teams meeting recording");
            throw new VideoConferencingException(
                "Failed to get Teams meeting recording", ex);
        }
    }

    private async Task<string> CreateTeamsChatThread(string topic)
    {
        var chat = new Chat
        {
            ChatType = ChatType.Meeting,
            Topic = topic,
            Members = new ChatMembersCollectionPage()
        };

        var createdChat = await _graphClient.Chats
            .Request()
            .AddAsync(chat);

        return createdChat.Id;
    }

    private MeetingStatus GetMeetingStatus(OnlineMeeting meeting)
    {
        if (meeting.CanceledDateTime.HasValue)
            return MeetingStatus.Cancelled;

        var now = DateTime.UtcNow;
        if (meeting.StartDateTime > now)
            return MeetingStatus.Scheduled;
        if (meeting.EndDateTime < now)
            return MeetingStatus.Completed;
        return MeetingStatus.InProgress;
    }
}