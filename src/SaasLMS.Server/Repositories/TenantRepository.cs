using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetBySubdomainAsync(string subdomain);
    Task<Tenant?> GetByCustomDomainAsync(string domain);
}

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.Subdomain == subdomain);
    }

    public async Task<Tenant?> GetByCustomDomainAsync(string domain)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.CustomDomain == domain);
    }
}