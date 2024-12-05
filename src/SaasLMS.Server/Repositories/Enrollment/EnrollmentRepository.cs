using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models.Enrollment;

namespace SaasLMS.Server.Repositories.Enrollment;

public class EnrollmentRepository : Repository<Shared.Models.Enrollment.Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Shared.Models.Enrollment.Enrollment>> GetUserEnrollmentsAsync(Guid userId)
    {
        return await DbSet
            .Where(e => e.UserId == userId)
            .Include(e => e.LessonCompletions)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }

    public async Task<Shared.Models.Enrollment.Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId)
    {
        return await DbSet
            .Include(e => e.LessonCompletions)
            .Include(e => e.QuizAttempts)
            .Include(e => e.AssignmentSubmissions)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);
    }

    public async Task<IEnumerable<Shared.Models.Enrollment.Enrollment>> GetCourseEnrollmentsAsync(Guid courseId)
    {
        return await DbSet
            .Where(e => e.CourseId == courseId)
            .Include(e => e.LessonCompletions)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateEnrollmentProgressAsync(Guid enrollmentId, float progress)
    {
        var enrollment = await DbSet.FindAsync(enrollmentId);
        if (enrollment == null) return false;

        enrollment.Progress = progress;
        enrollment.LastAccessedAt = DateTime.UtcNow;

        if (progress >= 100 && !enrollment.CompletedAt.HasValue)
        {
            enrollment.CompletedAt = DateTime.UtcNow;
            enrollment.Status = EnrollmentStatus.Completed;
        }

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteEnrollmentAsync(Guid enrollmentId)
    {
        var enrollment = await DbSet.FindAsync(enrollmentId);
        if (enrollment == null) return false;

        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.CompletedAt = DateTime.UtcNow;
        enrollment.Progress = 100;

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IssueEnrollmentCertificateAsync(Guid enrollmentId, string certificateUrl)
    {
        var enrollment = await DbSet.FindAsync(enrollmentId);
        if (enrollment == null) return false;

        enrollment.IsCertificateIssued = true;
        enrollment.CertificateUrl = certificateUrl;
        enrollment.CertificateIssuedAt = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LessonCompletion>> GetLessonCompletionsAsync(Guid enrollmentId)
    {
        return await Context.Set<LessonCompletion>()
            .Where(lc => lc.EnrollmentId == enrollmentId)
            .OrderBy(lc => lc.StartedAt)
            .ToListAsync();
    }
}