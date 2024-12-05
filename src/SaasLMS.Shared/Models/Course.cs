namespace SaasLMS.Shared.Models;

public class Course
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Module> Modules { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
}