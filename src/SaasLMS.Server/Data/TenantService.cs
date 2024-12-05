using Microsoft.EntityFrameworkCore;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Data;

public class TenantService : ITenantService
{
    private Tenant? _currentTenant;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;

    public TenantService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public Tenant CurrentTenant => _currentTenant ?? 
        throw new InvalidOperationException("Tenant is not set");

    public async Task<Tenant> GetTenantAsync(string identifier)
    {
        var tenant = await _dbContext.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == identifier || t.CustomDomain == identifier);

        if (tenant == null)
        {
            throw new InvalidOperationException($"Tenant not found for identifier: {identifier}");
        }

        return tenant;
    }

    public void SetCurrentTenant(Tenant tenant)
    {
        _currentTenant = tenant;
    }
}