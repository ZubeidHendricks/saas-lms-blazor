namespace SaasLMS.Core.Integration.VideoConferencing.Services;

public interface IRecordingTranscriptionService
{
    Task<string> GetTranscriptAsync(string recordingId);
    Task<bool> GenerateTranscriptAsync(string recordingId);
    Task<List<TranscriptSegment>> GetSearchableTranscriptAsync(string recordingId);
    Task<bool> SaveTranscriptAsync(string recordingId, string transcript);
}

public class RecordingTranscriptionService : IRecordingTranscriptionService
{
    private readonly IAzureSpeechService _speechService;
    private readonly IVideoConferencingFactory _videoConferencingFactory;
    private readonly IDbContext _dbContext;
    private readonly ILogger<RecordingTranscriptionService> _logger;

    public RecordingTranscriptionService(
        IAzureSpeechService speechService,
        IVideoConferencingFactory videoConferencingFactory,
        IDbContext dbContext,
        ILogger<RecordingTranscriptionService> logger)
    {
        _speechService = speechService;
        _videoConferencingFactory = videoConferencingFactory;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> GenerateTranscriptAsync(string recordingId)
    {
        try
        {
            // Get recording details
            var recording = await _dbContext.Recordings
                .FirstOrDefaultAsync(r => r.Id == recordingId);

            if (recording == null)
                throw new NotFoundException("Recording not found");

            // Get the video conferencing service
            var service = _videoConferencingFactory.GetService(recording.Platform);
            var recordingDetails = await service.GetRecordingAsync(recordingId);

            // Download audio stream
            var audioStream = await DownloadAudioStreamAsync(recordingDetails.DownloadUrl);

            // Generate transcript using Azure Speech Service
            var transcript = await _speechService.TranscribeAudioAsync(audioStream);

            // Save transcript
            await SaveTranscriptAsync(recordingId, transcript);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript for recording {RecordingId}", recordingId);
            return false;
        }
    }

    public async Task<string> GetTranscriptAsync(string recordingId)
    {
        var transcript = await _dbContext.Transcripts
            .FirstOrDefaultAsync(t => t.RecordingId == recordingId);

        return transcript?.Content;
    }

    public async Task<List<TranscriptSegment>> GetSearchableTranscriptAsync(string recordingId)
    {
        var transcript = await _dbContext.Transcripts
            .Include(t => t.Segments)
            .FirstOrDefaultAsync(t => t.RecordingId == recordingId);

        return transcript?.Segments.OrderBy(s => s.StartTime).ToList() ?? new List<TranscriptSegment>();
    }

    public async Task<bool> SaveTranscriptAsync(string recordingId, string transcript)
    {
        try
        {
            var existingTranscript = await _dbContext.Transcripts
                .FirstOrDefaultAsync(t => t.RecordingId == recordingId);

            if (existingTranscript != null)
            {
                existingTranscript.Content = transcript;
                existingTranscript.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _dbContext.Transcripts.Add(new RecordingTranscript
                {
                    RecordingId = recordingId,
                    Content = transcript,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving transcript for recording {RecordingId}", recordingId);
            return false;
        }
    }

    private async Task<Stream> DownloadAudioStreamAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStreamAsync();
    }
}