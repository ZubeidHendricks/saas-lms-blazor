using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardDTO> GetDashboardDataAsync(string timeframe = "week");
    Task<List<RecentActivityDTO>> GetRecentActivitiesAsync(int count = 10);
    Task<Dictionary<string, object>> GetQuickStatsAsync();
    Task<List<TrendingCourseDTO>> GetTrendingCoursesAsync(int count = 5);
    Task<List<TopInstructorDTO>> GetTopInstructorsAsync(int count = 5);
    Task<RevenueOverviewDTO> GetRevenueOverviewAsync(string timeframe = "month");
}