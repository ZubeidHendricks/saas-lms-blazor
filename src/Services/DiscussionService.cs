public class DiscussionService : IDiscussionService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public DiscussionService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<List<Discussion>> GetDiscussionsAsync(int courseId)
    {
        try
        {
            var discussions = await _httpClient.GetFromJsonAsync<List<Discussion>>($"api/discussions/{courseId}");
            await _localStorage.SetItemAsync($"discussions_{courseId}", discussions);
            return discussions;
        }
        catch
        {
            return await _localStorage.GetItemAsync<List<Discussion>>($"discussions_{courseId}") ?? new List<Discussion>();
        }
    }
}