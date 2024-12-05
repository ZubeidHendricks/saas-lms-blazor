using SaasLMS.Server.Repositories.Enrollment;
using SaasLMS.Server.Repositories.CourseStructure;
using SaasLMS.Server.Services.Payment;
using SaasLMS.Server.Services.Certificate;
using SaasLMS.Shared.Models.Enrollment;
using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Enrollment;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IPaymentService _paymentService;
    private readonly ICertificateService _certificateService;
    private readonly ITenantService _tenantService;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IPaymentService paymentService,
        ICertificateService certificateService,
        ITenantService tenantService)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _paymentService = paymentService;
        _certificateService = certificateService;
        _tenantService = tenantService;
    }

    public async Task<EnrollmentDTO> EnrollInCourseAsync(Guid userId, Guid courseId, PaymentInfo paymentInfo)
    {
        // Check if already enrolled
        var existingEnrollment = await _enrollmentRepository.FindAsync(e => 
            e.UserId == userId && e.CourseId == courseId);

        if (existingEnrollment.Any())
        {
            throw new InvalidOperationException("User is already enrolled in this course");
        }

        // Get course details
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found");
        }

        // Process payment if course is not free
        Transaction? transaction = null;
        if (!course.IsFree)
        {
            transaction = await _paymentService.ProcessPaymentAsync(new PaymentRequest
            {
                Amount = course.Price,
                UserId = userId,
                CourseId = courseId,
                PaymentMethod = paymentInfo.PaymentMethod,
                PaymentToken = paymentInfo.PaymentToken
            });
        }

        // Create enrollment
        var enrollment = new Shared.Models.Enrollment.Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            Status = EnrollmentStatus.Enrolled,
            PricePaid = transaction?.Amount ?? 0,
            PaymentMethod = transaction?.PaymentMethod ?? "free",
            TransactionId = transaction?.TransactionId ?? string.Empty,
            EnrolledAt = DateTime.UtcNow,
            Progress = 0,
            CompletedLessons = 0,
            TotalLessons = course.TotalLessons
        };

        await _enrollmentRepository.AddAsync(enrollment);

        return MapToDTO(enrollment);
    }

    public async Task<EnrollmentDTO?> GetEnrollmentAsync(Guid enrollmentId)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentWithDetailsAsync(enrollmentId);
        return enrollment != null ? MapToDTO(enrollment) : null;
    }

    public async Task<IEnumerable<EnrollmentDTO>> GetUserEnrollmentsAsync(Guid userId)
    {
        var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(userId);
        return enrollments.Select(MapToDTO);
    }

    public async Task<ProgressDTO> GetCourseProgressAsync(Guid enrollmentId)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentWithDetailsAsync(enrollmentId);
        if (enrollment == null)
        {
            throw new InvalidOperationException("Enrollment not found");
        }

        return new ProgressDTO
        {
            EnrollmentId = enrollmentId,
            Progress = enrollment.Progress,
            CompletedLessons = enrollment.CompletedLessons,
            TotalLessons = enrollment.TotalLessons,
            LastAccessedAt = enrollment.LastAccessedAt,
            LessonProgress = enrollment.LessonCompletions
                .Select(lc => new LessonProgressDTO
                {
                    LessonId = lc.LessonId,
                    Progress = lc.Progress,
                    Status = lc.Status.ToString(),
                    TimeSpent = lc.TimeSpent,
                    CompletedAt = lc.CompletedAt
                })
                .ToList()
        };
    }

    public async Task<bool> UpdateLessonProgressAsync(Guid enrollmentId, Guid lessonId, UpdateProgressDTO progress)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentWithDetailsAsync(enrollmentId);
        if (enrollment == null)
        {
            throw new InvalidOperationException("Enrollment not found");
        }

        var lessonCompletion = enrollment.LessonCompletions
            .FirstOrDefault(lc => lc.LessonId == lessonId);

        if (lessonCompletion == null)
        {
            lessonCompletion = new LessonCompletion
            {
                EnrollmentId = enrollmentId,
                LessonId = lessonId,
                StartedAt = DateTime.UtcNow
            };
            enrollment.LessonCompletions.Add(lessonCompletion);
        }

        lessonCompletion.Progress = progress.Progress;
        lessonCompletion.TimeSpent = progress.TimeSpent;
        lessonCompletion.LastAccessedAt = DateTime.UtcNow;

        if (progress.Progress >= 100)
        {
            lessonCompletion.Status = CompletionStatus.Completed;
            lessonCompletion.CompletedAt = DateTime.UtcNow;
        }

        // Update overall progress
        await UpdateEnrollmentProgressAsync(enrollment);

        return true;
    }

    private async Task UpdateEnrollmentProgressAsync(Shared.Models.Enrollment.Enrollment enrollment)
    {
        var completedLessons = enrollment.LessonCompletions
            .Count(lc => lc.Status == CompletionStatus.Completed);

        enrollment.CompletedLessons = completedLessons;
        enrollment.Progress = (float)completedLessons / enrollment.TotalLessons * 100;
        enrollment.LastAccessedAt = DateTime.UtcNow;

        if (enrollment.Progress >= 100 && !enrollment.CompletedAt.HasValue)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;

            // Generate certificate if enabled
            if (enrollment.Status == EnrollmentStatus.Completed)
            {
                await GenerateCertificateAsync(enrollment.Id);
            }
        }

        await _enrollmentRepository.UpdateAsync(enrollment);
    }

    // Continue in next section...