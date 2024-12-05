using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models.CourseStructure;

namespace SaasLMS.Server.Repositories.CourseStructure;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> GetCoursesForInstructorAsync(Guid instructorId)
    {
        return await DbSet
            .Where(c => c.InstructorId == instructorId)
            .Include(c => c.Sections)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10)
    {
        return await DbSet
            .Where(c => c.Status == CourseStatus.Published)
            .Include(c => c.Sections)
            .OrderByDescending(c => c.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Course?> GetCourseWithSectionsAsync(Guid courseId)
    {
        return await DbSet
            .Include(c => c.Sections)
            .ThenInclude(s => s.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }

    public async Task<Course?> GetCourseWithFullDetailsAsync(Guid courseId)
    {
        return await DbSet
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
                    .ThenInclude(l => l.Attachments)
            .Include(c => c.Reviews)
            .Include(c => c.Announcements)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }

    public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm, string? category = null)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.Title.Contains(searchTerm) ||
                c.Description.Contains(searchTerm) ||
                c.Requirements.Contains(searchTerm) ||
                c.Outcomes.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(c => c.Category == category);
        }

        return await query
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(50)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync()
    {
        return await DbSet
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.EnrollmentCount)
            .ThenByDescending(c => c.AverageRating)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetRelatedCoursesAsync(Guid courseId, int count = 5)
    {
        var course = await DbSet.FindAsync(courseId);
        if (course == null) return Enumerable.Empty<Course>();

        return await DbSet
            .Where(c => c.Id != courseId &&
                       c.Status == CourseStatus.Published &&
                       (c.Category == course.Category || c.SubCategory == course.SubCategory))
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetCourseStatisticsAsync(Guid courseId)
    {
        var course = await GetCourseWithFullDetailsAsync(courseId);
        if (course == null) return new Dictionary<string, int>();

        return new Dictionary<string, int>
        {
            { "totalEnrollments", course.EnrollmentCount },
            { "totalLessons", course.TotalLessons },
            { "totalReviews", course.Reviews.Count },
            { "totalAnnouncements", course.Announcements.Count },
            { "totalDuration", course.TotalDuration }
        };
    }

    public async Task<bool> UpdateCourseStatusAsync(Guid courseId, CourseStatus status)
    {
        var course = await DbSet.FindAsync(courseId);
        if (course == null) return false;

        course.Status = status;
        if (status == CourseStatus.Published && !course.PublishedAt.HasValue)
        {
            course.PublishedAt = DateTime.UtcNow;
        }

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsCourseEnrolledAsync(Guid courseId, Guid userId)
    {
        return await Context.Set<Enrollment>()
            .AnyAsync(e => e.CourseId == courseId && 
                          e.UserId == userId &&
                          e.Status != EnrollmentStatus.Dropped);
    }
}