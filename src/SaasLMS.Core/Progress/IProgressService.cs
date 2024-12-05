namespace SaasLMS.Core.Progress;

public interface IProgressService
{
    Task<StudentProgress> GetStudentProgressAsync(string studentId, Guid courseId);
    Task UpdateProgressAsync(StudentProgress progress);
    Task<bool> MarkContentCompletedAsync(string studentId, Guid contentId);
    Task<AssessmentAttempt> StartAssessmentAttemptAsync(string studentId, Guid assessmentId);
    Task<AssessmentAttempt> SubmitAssessmentAttemptAsync(Guid attemptId, List<QuestionResponse> responses);
    Task<List<StudentProgress>> GetCourseProgressAsync(Guid courseId);
}