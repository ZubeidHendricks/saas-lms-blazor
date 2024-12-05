namespace SaasLMS.Core.Courses;

public class CourseAssignment
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid ModuleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public int MaxScore { get; set; }
    public List<string> AllowedFileTypes { get; set; } = new();
    public int MaxSubmissions { get; set; }
    public bool RequiresGrading { get; set; }
}