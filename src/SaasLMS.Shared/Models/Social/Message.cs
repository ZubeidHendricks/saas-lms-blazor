namespace SaasLMS.Shared.Models.Social;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid? CourseId { get; set; }
    
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Attachments
    public List<MessageAttachment> Attachments { get; set; } = new();
    
    // Status
    public MessageStatus Status { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDeleted { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}