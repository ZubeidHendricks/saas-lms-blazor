namespace SaasLMS.Core.Courses;

public class Module
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public List<Content> Contents { get; set; } = new();
    public List<Assessment> Assessments { get; set; } = new();
    public ModuleSettings Settings { get; set; } = new();
}

public class ModuleSettings
{
    public bool RequireSequential { get; set; }
    public DateTime? UnlockAt { get; set; }
    public int? MinimumTimeSpent { get; set; }
    public List<Guid> Prerequisites { get; set; } = new();
}