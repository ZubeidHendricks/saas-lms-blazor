namespace SaasLMS.Shared.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserRole> Roles { get; set; } = new();
}