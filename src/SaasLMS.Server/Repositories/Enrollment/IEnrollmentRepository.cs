using SaasLMS.Shared.Models.Enrollment;

namespace SaasLMS.Server.Repositories.Enrollment;

public interface IEnrollmentRepository : IRepository<Shared.Models.Enrollment.Enrollment>
{
    Task<IEnumerable<Shared.Models.Enrollment.Enrollment>> GetUserEnrollmentsAsync(Guid userId);
    Task<Shared.Models.Enrollment.Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId);
    Task<IEnumerable<Shared.Models.Enrollment.Enrollment>> GetCourseEnrollmentsAsync(Guid courseId);
    Task<bool> UpdateEnrollmentProgressAsync(Guid enrollmentId, float progress);
    Task<bool> CompleteEnrollmentAsync(Guid enrollmentId);
    Task<bool> IssueEnrollmentCertificateAsync(Guid enrollmentId, string certificateUrl);
    Task<IEnumerable<LessonCompletion>> GetLessonCompletionsAsync(Guid enrollmentId);
}