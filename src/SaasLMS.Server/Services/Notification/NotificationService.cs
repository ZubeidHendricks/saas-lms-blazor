using Microsoft.AspNetCore.SignalR;
using SaasLMS.Server.Hubs;
using SaasLMS.Shared.Models;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly IHubContext<LearningHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;
    private readonly ITenantService _tenantService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<LearningHub> hubContext,
        INotificationRepository notificationRepository,
        ITenantService tenantService,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task SendCourseNotificationAsync(CourseNotificationDTO notification)
    {
        try
        {
            // Store notification
            var notificationEntity = new Notification
            {
                Type = NotificationType.Course,
                Title = notification.Title,
                Message = notification.Message,
                CourseId = notification.CourseId,
                TenantId = _tenantService.CurrentTenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notificationEntity);

            // Send real-time notification
            await _hubContext.Clients
                .Group($"course_{notification.CourseId}")
                .SendAsync("ReceiveNotification", notification);

            // Send push notifications if enabled
            if (notification.SendPush)
            {
                await SendPushNotificationsAsync(notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending course notification");
            throw;
        }
    }

    public async Task SendUserNotificationAsync(Guid userId, UserNotificationDTO notification)
    {
        try
        {
            var notificationEntity = new Notification
            {
                Type = NotificationType.User,
                Title = notification.Title,
                Message = notification.Message,
                UserId = userId,
                TenantId = _tenantService.CurrentTenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notificationEntity);

            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification);

            if (notification.SendPush)
            {
                await SendPushNotificationsToUserAsync(userId, notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending user notification");
            throw;
        }
    }

    public async Task SendTenantNotificationAsync(TenantNotificationDTO notification)
    {
        try
        {
            var notificationEntity = new Notification
            {
                Type = NotificationType.Tenant,
                Title = notification.Title,
                Message = notification.Message,
                TenantId = _tenantService.CurrentTenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notificationEntity);

            await _hubContext.Clients
                .Group($"tenant_{_tenantService.CurrentTenant.Id}")
                .SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending tenant notification");
            throw;
        }
    }

    public async Task SendLessonNotificationAsync(Guid lessonId, LessonNotificationDTO notification)
    {
        try
        {
            var notificationEntity = new Notification
            {
                Type = NotificationType.Lesson,
                Title = notification.Title,
                Message = notification.Message,
                LessonId = lessonId,
                TenantId = _tenantService.CurrentTenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notificationEntity);

            await _hubContext.Clients
                .Group($"lesson_{lessonId}")
                .SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending lesson notification");
            throw;
        }
    }

    public async Task NotifyInstructorAsync(Guid instructorId, InstructorNotificationDTO notification)
    {
        try
        {
            var notificationEntity = new Notification
            {
                Type = NotificationType.Instructor,
                Title = notification.Title,
                Message = notification.Message,
                UserId = instructorId,
                TenantId = _tenantService.CurrentTenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notificationEntity);

            await _hubContext.Clients
                .Group($"user_{instructorId}")
                .SendAsync("ReceiveInstructorNotification", notification);

            if (notification.SendPush)
            {
                await SendPushNotificationsToUserAsync(instructorId, notification);
            }

            // Send email notification if enabled
            if (notification.SendEmail)
            {
                await SendEmailNotificationAsync(instructorId, notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending instructor notification");
            throw;
        }
    }

    public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(Guid userId)
    {
        var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
        return notifications.Select(MapToDTO);
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    private async Task SendPushNotificationsAsync(NotificationDTO notification)
    {
        // Implement push notification logic (e.g., using Firebase Cloud Messaging)
    }

    private async Task SendPushNotificationsToUserAsync(Guid userId, NotificationDTO notification)
    {
        // Implement user-specific push notification logic
    }

    private async Task SendEmailNotificationAsync(Guid userId, NotificationDTO notification)
    {
        // Implement email notification logic
    }

    private NotificationDTO MapToDTO(Notification notification)
    {
        return new NotificationDTO
        {
            Id = notification.Id,
            Type = notification.Type.ToString(),
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}