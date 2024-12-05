namespace SaasLMS.Core.Messaging;

public interface IMessageService
{
    Task<List<Message>> GetMessagesAsync();
    Task<Message> GetMessageAsync(Guid messageId);
    Task<Message> SendMessageAsync(Message message);
    Task<bool> MarkAsReadAsync(Guid messageId);
    Task<bool> DeleteMessageAsync(Guid messageId);
    Task<bool> ArchiveMessageAsync(Guid messageId);
    Task<List<Message>> GetUnreadMessagesAsync();
}