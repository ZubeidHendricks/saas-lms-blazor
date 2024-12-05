using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetCourseWithModulesAsync(Guid id);
    Task<IEnumerable<Course>> GetCoursesByTenantAsync(Guid tenantId);
}

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetCourseWithModulesAsync(Guid id)
    {
        return await DbSet
            .Include(c => c.Modules)
            .ThenInclude(m => m.Contents)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetCoursesByTenantAsync(Guid tenantId)
    {
        return await DbSet
            .Where(c => c.TenantId == tenantId)
            .Include(c => c.Modules)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}