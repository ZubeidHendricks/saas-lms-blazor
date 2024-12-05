namespace SaasLMS.Core.Courses;

public interface ICourseService
{
    Task<Course> GetCourseAsync(Guid courseId);
    Task<Course> CreateCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task<List<Course>> GetCoursesByTenantAsync(Guid tenantId);
    Task<List<Course>> GetCoursesByInstructorAsync(string instructorId);
    Task<bool> DeleteCourseAsync(Guid courseId);
    Task<bool> PublishCourseAsync(Guid courseId);
    Task<bool> ArchiveCourseAsync(Guid courseId);
}