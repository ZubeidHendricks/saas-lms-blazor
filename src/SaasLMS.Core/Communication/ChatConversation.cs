namespace SaasLMS.Core.Communication;

public class ChatConversation
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ConversationType Type { get; set; }
    public List<string> Participants { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }
    public bool IsActive { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
}

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public List<string> ReadBy { get; set; } = new();
    public List<MessageAttachment> Attachments { get; set; } = new();
}

public enum ConversationType
{
    OneToOne,
    Group,
    CourseDiscussion
}