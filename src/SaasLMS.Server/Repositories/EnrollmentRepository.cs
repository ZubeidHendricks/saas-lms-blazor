using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId);
    Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid userId, Guid courseId);
}

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId)
    {
        return await DbSet
            .Where(e => e.UserId == userId)
            .Include(e => e.ProgressRecords)
            .OrderByDescending(e => e.StartedAt)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid userId, Guid courseId)
    {
        return await DbSet
            .Include(e => e.ProgressRecords)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }
}