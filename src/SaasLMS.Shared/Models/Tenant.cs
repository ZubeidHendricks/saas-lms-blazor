namespace SaasLMS.Shared.Models;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string? CustomDomain { get; set; }
    public TenantPlan Plan { get; set; } = TenantPlan.Basic;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public ThemeConfiguration ThemeConfig { get; set; } = new();
    public long StorageQuota { get; set; }
    public long StorageUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}