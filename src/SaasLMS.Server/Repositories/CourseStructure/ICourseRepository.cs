using SaasLMS.Shared.Models.CourseStructure;

namespace SaasLMS.Server.Repositories.CourseStructure;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetCoursesForInstructorAsync(Guid instructorId);
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10);
    Task<Course?> GetCourseWithSectionsAsync(Guid courseId);
    Task<Course?> GetCourseWithFullDetailsAsync(Guid courseId);
    Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm, string? category = null);
    Task<IEnumerable<Course>> GetFeaturedCoursesAsync();
    Task<IEnumerable<Course>> GetRelatedCoursesAsync(Guid courseId, int count = 5);
    Task<Dictionary<string, int>> GetCourseStatisticsAsync(Guid courseId);
    Task<bool> UpdateCourseStatusAsync(Guid courseId, CourseStatus status);
    Task<bool> IsCourseEnrolledAsync(Guid courseId, Guid userId);
}