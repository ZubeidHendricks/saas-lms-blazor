namespace SaasLMS.Core.MultiTenancy;

public class TenantService : ITenantService
{
    private readonly IDbContext _dbContext;
    private readonly ICache _cache;

    public TenantService(IDbContext dbContext, ICache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<TenantInfo> GetTenantAsync(string subdomain)
    {
        var cacheKey = $"tenant:{subdomain}";
        var tenant = await _cache.GetAsync<TenantInfo>(cacheKey);
        
        if (tenant == null)
        {
            tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.SubDomain == subdomain && t.IsActive);

            if (tenant != null)
            {
                await _cache.SetAsync(cacheKey, tenant, TimeSpan.FromMinutes(30));
            }
        }

        return tenant;
    }

    public async Task<TenantInfo> CreateTenantAsync(TenantInfo tenant)
    {
        tenant.Id = Guid.NewGuid();
        tenant.CreatedAt = DateTime.UtcNow;
        tenant.IsActive = true;

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        await _cache.SetAsync($"tenant:{tenant.SubDomain}", tenant, TimeSpan.FromMinutes(30));

        return tenant;
    }

    public async Task UpdateTenantAsync(TenantInfo tenant)
    {
        _dbContext.Tenants.Update(tenant);
        await _dbContext.SaveChangesAsync();
        await _cache.SetAsync($"tenant:{tenant.SubDomain}", tenant, TimeSpan.FromMinutes(30));
    }

    public async Task<bool> IsDomainAvailableAsync(string subdomain)
    {
        return !await _dbContext.Tenants.AnyAsync(t => t.SubDomain == subdomain);
    }

    public async Task<List<string>> GetActiveFeaturesAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        return tenant.Plan switch
        {
            TenantPlan.Free => new List<string> { "basic_courses", "basic_analytics" },
            TenantPlan.Basic => new List<string> { "basic_courses", "basic_analytics", "assignments", "quizzes" },
            TenantPlan.Professional => new List<string> { "basic_courses", "advanced_analytics", "assignments", "quizzes", "certificates", "api_access" },
            TenantPlan.Enterprise => new List<string> { "basic_courses", "advanced_analytics", "assignments", "quizzes", "certificates", "api_access", "white_label", "sso", "custom_domain" },
            _ => new List<string>()
        };
    }
}