namespace SaasLMS.Core.Communication;

public class NotificationService : INotificationService
{
    private readonly IDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IDbContext dbContext,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<Notification> SendNotificationAsync(Notification notification)
    {
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.UtcNow;

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        // Send real-time notification
        await _hubContext.Clients
            .User(notification.UserId)
            .SendAsync("ReceiveNotification", notification);

        return notification;
    }

    public async Task SendBulkNotificationsAsync(
        List<string> userIds,
        string title,
        string message,
        NotificationType type)
    {
        var notifications = userIds.Select(userId => new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _dbContext.Notifications.AddRange(notifications);
        await _dbContext.SaveChangesAsync();

        // Send real-time notifications
        var tasks = notifications.Select(n =>
            _hubContext.Clients
                .User(n.UserId)
                .SendAsync("ReceiveNotification", n));

        await Task.WhenAll(tasks);
    }

    public async Task<List<Notification>> GetNotificationsAsync(
        string userId,
        bool unreadOnly = false)
    {
        var query = _dbContext.Notifications
            .Where(n => n.UserId == userId);

        if (unreadOnly)
            query = query.Where(n => !n.ReadAt.HasValue);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _dbContext.Notifications
            .FindAsync(notificationId);

        if (notification == null)
            return false;

        notification.ReadAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId)
    {
        var notification = await _dbContext.Notifications
            .FindAsync(notificationId);

        if (notification == null)
            return false;

        _dbContext.Notifications.Remove(notification);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}