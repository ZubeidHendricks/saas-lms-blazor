namespace SaasLMS.Shared.Models.Social;

public class MessageThread
{
    public Guid Id { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    
    // Last message info
    public string LastMessagePreview { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public Guid LastMessageSenderId { get; set; }
    
    // Status for each participant
    public Dictionary<Guid, ThreadStatus> ParticipantStatus { get; set; } = new();
    
    // Messages in thread
    public virtual List<Message> Messages { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}