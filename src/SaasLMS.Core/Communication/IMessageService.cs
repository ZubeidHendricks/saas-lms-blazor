namespace SaasLMS.Core.Communication;

public interface IMessageService
{
    Task<Message> SendMessageAsync(Message message);
    Task<List<Message>> GetMessagesAsync(string userId, MessageFilter filter);
    Task<bool> MarkAsReadAsync(Guid messageId, string userId);
    Task<bool> ArchiveMessageAsync(Guid messageId, string userId);
    Task<bool> DeleteMessageAsync(Guid messageId, string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task<List<Message>> GetAnnouncementsAsync(Guid courseId);
}

public class MessageFilter
{
    public MessageType? Type { get; set; }
    public MessageStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IncludeArchived { get; set; }
    public string SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}