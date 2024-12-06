namespace SaasLMS.Core.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly IDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        IDbContext dbContext,
        ICacheService cacheService,
        ILogger<AnalyticsService> logger)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<DashboardStats> GetInstructorDashboardStatsAsync(string instructorId)
    {
        var cacheKey = $"instructor_stats_{instructorId}";
        var stats = await _cacheService.GetAsync<DashboardStats>(cacheKey);

        if (stats == null)
        {
            stats = new DashboardStats
            {
                TotalStudents = await _dbContext.CourseEnrollments
                    .Where(e => e.Course.InstructorId == instructorId)
                    .Select(e => e.StudentId)
                    .Distinct()
                    .CountAsync(),

                TotalRevenue = await _dbContext.CourseEnrollments
                    .Where(e => e.Course.InstructorId == instructorId)
                    .SumAsync(e => e.AmountPaid),

                ActiveCourses = await _dbContext.Courses
                    .Where(c => c.InstructorId == instructorId && c.Status == CourseStatus.Published)
                    .CountAsync(),

                CompletionRate = await CalculateCompletionRateAsync(instructorId),

                RecentEnrollments = await _dbContext.CourseEnrollments
                    .Where(e => e.Course.InstructorId == instructorId)
                    .OrderByDescending(e => e.EnrolledAt)
                    .Take(10)
                    .Select(e => new EnrollmentInfo
                    {
                        StudentName = e.Student.Name,
                        CourseName = e.Course.Title,
                        EnrolledAt = e.EnrolledAt
                    })
                    .ToListAsync(),

                PopularCourses = await _dbContext.Courses
                    .Where(c => c.InstructorId == instructorId)
                    .OrderByDescending(c => c.Enrollments.Count)
                    .Take(5)
                    .Select(c => new CourseStats
                    {
                        CourseId = c.Id,
                        Title = c.Title,
                        EnrollmentCount = c.Enrollments.Count,
                        CompletionCount = c.Enrollments.Count(e => e.CompletedAt.HasValue),
                        AverageRating = c.Reviews.Average(r => r.Rating) ?? 0
                    })
                    .ToListAsync()
            };

            await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(15));
        }

        return stats;
    }

    public async Task<List<RevenueStats>> GetRevenueStatsAsync(
        string instructorId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var revenues = await _dbContext.CourseEnrollments
            .Where(e => e.Course.InstructorId == instructorId &&
                       e.EnrolledAt >= startDate &&
                       e.EnrolledAt <= endDate)
            .GroupBy(e => e.EnrolledAt.Date)
            .Select(g => new RevenueStats
            {
                Date = g.Key,
                Revenue = g.Sum(e => e.AmountPaid),
                EnrollmentCount = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        return revenues;
    }

    public async Task<List<StudentEngagement>> GetStudentEngagementStatsAsync(
        Guid courseId)
    {
        var engagements = await _dbContext.StudentProgress
            .Where(p => p.CourseId == courseId)
            .Select(p => new StudentEngagement
            {
                StudentId = p.StudentId,
                StudentName = p.Student.Name,
                LastAccessedAt = p.LastAccessedAt,
                CompletionPercentage = p.CompletionPercentage,
                TimeSpent = p.TimeSpent,
                AssessmentScores = p.AssessmentAttempts
                    .Where(a => a.CompletedAt.HasValue)
                    .Select(a => a.Score)
                    .ToList()
            })
            .ToListAsync();

        return engagements;
    }

    private async Task<double> CalculateCompletionRateAsync(string instructorId)
    {
        var enrollments = await _dbContext.CourseEnrollments
            .Where(e => e.Course.InstructorId == instructorId)
            .Select(e => new
            {
                IsCompleted = e.CompletedAt.HasValue
            })
            .ToListAsync();

        if (!enrollments.Any())
            return 0;

        return enrollments.Count(e => e.IsCompleted) / (double)enrollments.Count * 100;
    }
}