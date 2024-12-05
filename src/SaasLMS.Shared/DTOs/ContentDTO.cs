namespace SaasLMS.Shared.DTOs;

public record ContentDTO
{
    public Guid Id { get; init; }
    public Guid ModuleId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ContentData { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record CreateContentDTO
{
    public Guid ModuleId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ContentData { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
}

public record UpdateContentDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentData { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string Status { get; init; } = string.Empty;
}