namespace SaasLMS.Shared.DTOs;

public record CourseDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? PublishedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<ModuleDTO> Modules { get; init; } = new();
    public int EnrollmentCount { get; init; }
    public float CompletionRate { get; init; }
}

public record CreateCourseDTO
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record UpdateCourseDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}