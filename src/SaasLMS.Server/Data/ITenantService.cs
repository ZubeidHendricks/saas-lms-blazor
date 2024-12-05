using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Data;

public interface ITenantService
{
    Tenant CurrentTenant { get; }
    Task<Tenant> GetTenantAsync(string identifier);
    void SetCurrentTenant(Tenant tenant);
}