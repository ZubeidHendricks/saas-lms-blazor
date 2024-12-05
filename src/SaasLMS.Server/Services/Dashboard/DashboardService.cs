using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<DashboardService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<DashboardDTO> GetDashboardDataAsync(string timeframe = "week")
    {
        try
        {
            var tenantId = _tenantService.CurrentTenant.Id;
            var startDate = GetStartDate(timeframe);

            var courses = await _context.Courses
                .Where(c => c.TenantId == tenantId)
                .Include(c => c.Enrollments)
                .ToListAsync();

            var transactions = await _context.Transactions
                .Where(t => t.CreatedAt >= startDate)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();

            return new DashboardDTO
            {
                QuickStats = new QuickStatsDTO
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.LastLoginAt >= startDate),
                    TotalCourses = courses.Count,
                    TotalRevenue = transactions.Sum(t => t.Amount)
                },
                RevenueOverview = await GetRevenueOverviewAsync(timeframe),
                TrendingCourses = await GetTrendingCoursesAsync(),
                TopInstructors = await GetTopInstructorsAsync(),
                RecentActivities = await GetRecentActivitiesAsync()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            throw;
        }
    }

    public async Task<List<RecentActivityDTO>> GetRecentActivitiesAsync(int count = 10)
    {
        var tenantId = _tenantService.CurrentTenant.Id;

        var activities = await _context.AnalyticsEvents
            .Where(e => e.TenantId == tenantId)
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .Select(e => new RecentActivityDTO
            {
                Type = e.EventName,
                Description = e.Properties,
                Timestamp = e.Timestamp
            })
            .ToListAsync();

        return activities;
    }

    public async Task<Dictionary<string, object>> GetQuickStatsAsync()
    {
        var tenantId = _tenantService.CurrentTenant.Id;
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);

        var stats = new Dictionary<string, object>();

        // Users
        var users = await _context.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();

        stats.Add("totalUsers", users.Count);
        stats.Add("activeUsers", users.Count(u => u.LastLoginAt >= thirtyDaysAgo));
        stats.Add("newUsers", users.Count(u => u.CreatedAt >= thirtyDaysAgo));

        // Courses
        var courses = await _context.Courses
            .Where(c => c.TenantId == tenantId)
            .Include(c => c.Enrollments)
            .ToListAsync();

        stats.Add("totalCourses", courses.Count);
        stats.Add("publishedCourses", courses.Count(c => c.Status == CourseStatus.Published));
        stats.Add("totalEnrollments", courses.Sum(c => c.Enrollments.Count));

        // Revenue
        var transactions = await _context.Transactions
            .Where(t => t.CreatedAt >= thirtyDaysAgo)
            .ToListAsync();

        stats.Add("monthlyRevenue", transactions.Sum(t => t.Amount));
        stats.Add("averageOrderValue", transactions.Any() 
            ? transactions.Average(t => t.Amount) 
            : 0);

        return stats;
    }

    public async Task<List<TrendingCourseDTO>> GetTrendingCoursesAsync(int count = 5)
    {
        var tenantId = _tenantService.CurrentTenant.Id;
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var trendingCourses = await _context.Courses
            .Where(c => c.TenantId == tenantId)
            .Include(c => c.Enrollments)
            .Include(c => c.Reviews)
            .OrderByDescending(c => c.Enrollments.Count(e => e.EnrolledAt >= thirtyDaysAgo))
            .Take(count)
            .Select(c => new TrendingCourseDTO
            {
                CourseId = c.Id,
                Title = c.Title,
                EnrollmentCount = c.Enrollments.Count,
                RecentEnrollments = c.Enrollments.Count(e => e.EnrolledAt >= thirtyDaysAgo),
                Rating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
                Revenue = c.Enrollments.Sum(e => e.PricePaid)
            })
            .ToListAsync();

        return trendingCourses;
    }

    public async Task<List<TopInstructorDTO>> GetTopInstructorsAsync(int count = 5)
    {
        var tenantId = _tenantService.CurrentTenant.Id;
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var topInstructors = await _context.Users
            .Where(u => u.TenantId == tenantId && u.Roles.Any(r => r.Name == "Instructor"))
            .Include(u => u.Courses)
                .ThenInclude(c => c.Enrollments)
            .Include(u => u.Courses)
                .ThenInclude(c => c.Reviews)
            .OrderByDescending(u => u.Courses.SelectMany(c => c.Enrollments).Count())
            .Take(count)
            .Select(u => new TopInstructorDTO
            {
                InstructorId = u.Id,
                Name = $"{u.FirstName} {u.LastName}",
                TotalCourses = u.Courses.Count,
                TotalStudents = u.Courses.SelectMany(c => c.Enrollments).Count(),
                AverageRating = u.Courses.SelectMany(c => c.Reviews).Any() 
                    ? u.Courses.SelectMany(c => c.Reviews).Average(r => r.Rating)
                    : 0,
                Revenue = u.Courses.SelectMany(c => c.Enrollments).Sum(e => e.PricePaid)
            })
            .ToListAsync();

        return topInstructors;
    }

    public async Task<RevenueOverviewDTO> GetRevenueOverviewAsync(string timeframe = "month")
    {
        var startDate = GetStartDate(timeframe);
        var transactions = await _context.Transactions
            .Where(t => t.CreatedAt >= startDate)
            .ToListAsync();

        return new RevenueOverviewDTO
        {
            TotalRevenue = transactions.Sum(t => t.Amount),
            RevenueByDay = GetRevenueByDay(transactions),
            RevenueBySource = GetRevenueBySource(transactions),
            AverageOrderValue = transactions.Any() 
                ? transactions.Average(t => t.Amount)
                : 0
        };
    }

    private DateTime GetStartDate(string timeframe)
    {
        var now = DateTime.UtcNow;
        return timeframe.ToLower() switch
        {
            "day" => now.AddDays(-1),
            "week" => now.AddDays(-7),
            "month" => now.AddMonths(-1),
            "quarter" => now.AddMonths(-3),
            "year" => now.AddYears(-1),
            _ => now.AddDays(-7)
        };
    }

    private Dictionary<string, decimal> GetRevenueByDay(List<Transaction> transactions)
    {
        return transactions
            .GroupBy(t => t.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key.ToString("yyyy-MM-dd"),
                g => g.Sum(t => t.Amount));
    }

    private Dictionary<string, decimal> GetRevenueBySource(List<Transaction> transactions)
    {
        return transactions
            .GroupBy(t => t.Type.ToString())
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Amount));
    }
}