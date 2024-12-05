namespace SaasLMS.Core.MultiTenancy;

public interface ITenantService
{
    Task<TenantInfo> GetTenantAsync(string subdomain);
    Task<TenantInfo> CreateTenantAsync(TenantInfo tenant);
    Task UpdateTenantAsync(TenantInfo tenant);
    Task<bool> IsDomainAvailableAsync(string subdomain);
    Task<List<string>> GetActiveFeaturesAsync(Guid tenantId);
}