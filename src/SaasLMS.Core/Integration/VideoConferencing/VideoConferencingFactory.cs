namespace SaasLMS.Core.Integration.VideoConferencing;

public class VideoConferencingFactory : IVideoConferencingFactory
{
    private readonly IServiceProvider _serviceProvider;

    public VideoConferencingFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IVideoConferencingService GetService(MeetingPlatform platform)
    {
        return platform switch
        {
            MeetingPlatform.Zoom => _serviceProvider.GetRequiredService<ZoomService>(),
            MeetingPlatform.Teams => _serviceProvider.GetRequiredService<TeamsService>(),
            _ => throw new ArgumentException($"Unsupported platform: {platform}")
        };
    }
}