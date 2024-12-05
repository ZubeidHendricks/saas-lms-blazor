public class AnalyticsService : IAnalyticsService
{
    // ... previous code ...

    private float CalculateCompletionRate(List<Enrollment> enrollments)
    {
        if (!enrollments.Any()) return 0;
        return (float)enrollments.Count(e => e.Status == EnrollmentStatus.Completed) / enrollments.Count * 100;
    }

    private float CalculateAverageProgress(List<Enrollment> enrollments)
    {
        if (!enrollments.Any()) return 0;
        return enrollments.Average(e => e.Progress);
    }

    private Dictionary<string, int> GetEnrollmentTrend(List<Enrollment> enrollments)
    {
        return enrollments
            .GroupBy(e => e.EnrolledAt.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Count());
    }

    private Dictionary<string, float> GetCompletionsByModule(List<LessonCompletion> completions)
    {
        return completions
            .GroupBy(c => c.Lesson.Module.Title)
            .ToDictionary(
                g => g.Key,
                g => (float)g.Count(c => c.Status == CompletionStatus.Completed) / g.Count() * 100);
    }

    private Dictionary<string, TimeSpan> GetTimeSpentDistribution(List<LessonCompletion> completions)
    {
        return completions
            .GroupBy(c => c.Lesson.Type.ToString())
            .ToDictionary(
                g => g.Key,
                g => TimeSpan.FromSeconds(g.Sum(c => c.TimeSpent)));
    }

    private EngagementMetricsDTO GetEngagementMetrics(List<LessonCompletion> completions)
    {
        return new EngagementMetricsDTO
        {
            AverageTimePerLesson = TimeSpan.FromSeconds(
                completions.Any() ? completions.Average(c => c.TimeSpent) : 0),
            TotalTimeSpent = TimeSpan.FromSeconds(completions.Sum(c => c.TimeSpent)),
            CompletionRate = completions.Any() 
                ? (float)completions.Count(c => c.Status == CompletionStatus.Completed) / completions.Count * 100
                : 0,
            ActiveDays = completions
                .Select(c => c.LastAccessedAt.Date)
                .Distinct()
                .Count()
        };
    }

    private float CalculateAverageInstructorRating(List<Course> courses)
    {
        var allReviews = courses.SelectMany(c => c.Reviews);
        return allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;
    }

    private Dictionary<string, decimal> GetRevenueByMonth(List<InstructorEarning> earnings)
    {
        return earnings
            .GroupBy(e => e.CreatedAt.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.EarnedAmount));
    }

    private List<CoursePerformanceDTO> GetCoursePerformance(List<Course> courses)
    {
        return courses.Select(c => new CoursePerformanceDTO
        {
            CourseId = c.Id,
            Title = c.Title,
            EnrollmentCount = c.Enrollments.Count,
            CompletionRate = CalculateCompletionRate(c.Enrollments.ToList()),
            AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
            Revenue = c.Enrollments.Sum(e => e.PricePaid)
        }).ToList();
    }

    private StudentEngagementDTO GetStudentEngagement(List<Course> courses)
    {
        var allEnrollments = courses.SelectMany(c => c.Enrollments);
        var completedEnrollments = allEnrollments.Where(e => e.Status == EnrollmentStatus.Completed);

        return new StudentEngagementDTO
        {
            TotalStudents = allEnrollments.Select(e => e.UserId).Distinct().Count(),
            ActiveStudents = allEnrollments.Count(e => e.LastAccessedAt >= DateTime.UtcNow.AddDays(-30)),
            AverageCompletionTime = completedEnrollments.Any()
                ? TimeSpan.FromTicks((long)completedEnrollments
                    .Select(e => (e.CompletedAt!.Value - e.EnrolledAt).Ticks)
                    .Average())
                : TimeSpan.Zero,
            RetentionRate = CalculateRetentionRate(allEnrollments.ToList())
        };
    }

    private float CalculateRetentionRate(List<Enrollment> enrollments)
    {
        if (!enrollments.Any()) return 0;

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var olderEnrollments = enrollments.Where(e => e.EnrolledAt <= thirtyDaysAgo);
        
        if (!olderEnrollments.Any()) return 0;

        var activeUsers = olderEnrollments.Count(e => 
            e.LastAccessedAt >= thirtyDaysAgo || 
            e.Status == EnrollmentStatus.Completed);

        return (float)activeUsers / olderEnrollments.Count() * 100;
    }

    private UserGrowthDTO GetUserGrowth(List<User> users, DateTime startDate, DateTime endDate)
    {
        var newUsers = users.Count(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate);
        var previousPeriodUsers = users.Count(u => u.CreatedAt >= startDate.AddMonths(-1) && u.CreatedAt < startDate);

        return new UserGrowthDTO
        {
            TotalUsers = users.Count,
            NewUsers = newUsers,
            GrowthRate = previousPeriodUsers > 0
                ? ((float)newUsers - previousPeriodUsers) / previousPeriodUsers * 100
                : 0,
            GrowthByMonth = users
                .GroupBy(u => u.CreatedAt.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}