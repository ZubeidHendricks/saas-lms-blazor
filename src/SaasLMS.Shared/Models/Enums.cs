namespace SaasLMS.Shared.Models;

public enum TenantPlan
{
    Basic,
    Professional,
    Enterprise
}

public enum TenantStatus
{
    Active,
    Suspended,
    Deleted
}

public enum CourseStatus
{
    Draft,
    Published,
    Archived
}

public enum ModuleStatus
{
    Draft,
    Published,
    Archived
}

public enum ContentType
{
    Video,
    Document,
    Quiz,
    Assignment,
    Link,
    Embed
}

public enum ContentStatus
{
    Draft,
    Published,
    Archived
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}

public enum EnrollmentStatus
{
    Enrolled,
    Completed,
    Dropped
}

public enum ProgressStatus
{
    NotStarted,
    InProgress,
    Completed
}