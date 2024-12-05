namespace SaasLMS.Core.Courses;

public class Course
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string InstructorId { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<Module> Modules { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public CourseSettings Settings { get; set; } = new();
}

public enum CourseStatus
{
    Draft,
    Published,
    Archived
}

public class CourseSettings
{
    public bool IsPublic { get; set; }
    public bool AllowSelfEnrollment { get; set; }
    public bool RequireApproval { get; set; }
    public bool EnableDiscussions { get; set; }
    public int MaxStudents { get; set; }
    public DateTime? EnrollmentDeadline { get; set; }
}