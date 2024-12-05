using SaasLMS.Shared.Models.Enrollment;
using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Enrollment;

public interface IEnrollmentService
{
    Task<EnrollmentDTO> EnrollInCourseAsync(Guid userId, Guid courseId, PaymentInfo paymentInfo);
    Task<EnrollmentDTO?> GetEnrollmentAsync(Guid enrollmentId);
    Task<IEnumerable<EnrollmentDTO>> GetUserEnrollmentsAsync(Guid userId);
    Task<ProgressDTO> GetCourseProgressAsync(Guid enrollmentId);
    Task<bool> UpdateLessonProgressAsync(Guid enrollmentId, Guid lessonId, UpdateProgressDTO progress);
    Task<bool> CompleteLessonAsync(Guid enrollmentId, Guid lessonId);
    Task<QuizResultDTO> SubmitQuizAttemptAsync(Guid enrollmentId, Guid lessonId, QuizSubmissionDTO submission);
    Task<AssignmentSubmissionDTO> SubmitAssignmentAsync(Guid enrollmentId, Guid lessonId, AssignmentSubmissionDTO submission);
    Task<CertificateDTO> GenerateCertificateAsync(Guid enrollmentId);
}