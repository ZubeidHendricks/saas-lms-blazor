namespace SaasLMS.Shared.Models.CourseStructure;

public class Course
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Outcomes { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public string Level { get; set; } = "Beginner";
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string PreviewVideoUrl { get; set; } = string.Empty;
    
    // Pricing
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public bool IsFree { get; set; }
    
    // Course Settings
    public bool IsPrivate { get; set; }
    public bool IsDripContent { get; set; }
    public int DripInterval { get; set; } // in days
    public bool IsCertificateEnabled { get; set; }
    
    // Relationships
    public Guid InstructorId { get; set; }
    public virtual List<Section> Sections { get; set; } = new();
    public virtual List<Enrollment> Enrollments { get; set; } = new();
    public virtual List<CourseReview> Reviews { get; set; } = new();
    public virtual List<CourseAnnouncement> Announcements { get; set; } = new();
    
    // Statistics
    public int TotalDuration { get; set; } // in minutes
    public int TotalLessons { get; set; }
    public float AverageRating { get; set; }
    public int EnrollmentCount { get; set; }
    
    // Status
    public CourseStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}