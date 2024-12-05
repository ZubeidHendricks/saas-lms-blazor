namespace SaasLMS.Shared.DTOs;

public record ModuleDTO
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string Status { get; init; } = string.Empty;
    public List<ContentDTO> Contents { get; init; } = new();
}

public record CreateModuleDTO
{
    public Guid CourseId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
}

public record UpdateModuleDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string Status { get; init; } = string.Empty;
}