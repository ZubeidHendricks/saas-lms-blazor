public class ProgressTrackingService : IProgressTrackingService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ProgressTrackingService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<StudentProgress> GetProgressAsync(string studentId, int courseId)
    {
        try
        {
            var progress = await _httpClient.GetFromJsonAsync<StudentProgress>($"api/progress/{studentId}/{courseId}");
            await _localStorage.SetItemAsync($"progress_{studentId}_{courseId}", progress);
            return progress;
        }
        catch
        {
            return await _localStorage.GetItemAsync<StudentProgress>($"progress_{studentId}_{courseId}") ?? new StudentProgress { StudentId = studentId, CourseId = courseId };
        }
    }
}