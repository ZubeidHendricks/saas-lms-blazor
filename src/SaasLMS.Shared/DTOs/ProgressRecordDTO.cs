namespace SaasLMS.Shared.DTOs;

public record ProgressRecordDTO
{
    public Guid Id { get; init; }
    public Guid ContentId { get; init; }
    public string Status { get; init; } = string.Empty;
    public float Progress { get; init; }
    public int TimeSpent { get; init; }
    public DateTime LastAccessedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}

public record UpdateProgressDTO
{
    public Guid ContentId { get; init; }
    public float Progress { get; init; }
    public int TimeSpent { get; init; }
}