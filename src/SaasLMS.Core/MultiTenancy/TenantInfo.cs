namespace SaasLMS.Core.MultiTenancy;

public class TenantInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SubDomain { get; set; }
    public string ConnectionString { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public TenantPlan Plan { get; set; }
    public DateTime? TrialEndsAt { get; set; }
}

public enum TenantPlan
{
    Free,
    Basic,
    Professional,
    Enterprise
}