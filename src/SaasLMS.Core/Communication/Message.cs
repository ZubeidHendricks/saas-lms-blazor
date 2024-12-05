namespace SaasLMS.Core.Communication;

public class Message
{
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public MessageType Type { get; set; }
    public MessageStatus Status { get; set; }
    public MessagePriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public List<MessageAttachment> Attachments { get; set; } = new();
    public bool IsArchived { get; set; }
}

public enum MessageType
{
    Direct,
    Announcement,
    Notification,
    SystemMessage
}

public enum MessageStatus
{
    Draft,
    Sent,
    Delivered,
    Read,
    Failed
}

public enum MessagePriority
{
    Low,
    Normal,
    High,
    Urgent
}

public class MessageAttachment
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }
}