namespace SaasLMS.Shared.Models;

public class Content
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public ContentType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentData { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public ContentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}