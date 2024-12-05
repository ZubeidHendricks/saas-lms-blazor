namespace SaasLMS.Shared.DTOs;

public record EnrollmentDTO
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public float Progress { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public List<ProgressRecordDTO> ProgressRecords { get; init; } = new();
}

public record EnrollmentRequestDTO
{
    public Guid CourseId { get; init; }
}