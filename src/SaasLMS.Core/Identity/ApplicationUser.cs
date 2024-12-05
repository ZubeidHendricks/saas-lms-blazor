namespace SaasLMS.Core.Identity;

public class ApplicationUser : IdentityUser
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePicture { get; set; }
    public UserType Type { get; set; }
    public Dictionary<string, string> Preferences { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}

public enum UserType
{
    Student,
    Instructor,
    Admin,
    SuperAdmin
}