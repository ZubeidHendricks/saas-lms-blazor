namespace SaasLMS.Shared.Models.CourseStructure;

public class LessonAttachment
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}