public class DiscussionServiceTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<ILocalStorageService> _localStorageMock;
    private readonly DiscussionService _service;

    public DiscussionServiceTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _localStorageMock = new Mock<ILocalStorageService>();
        _service = new DiscussionService(_httpClientMock.Object, _localStorageMock.Object);
    }

    [Fact]
    public async Task GetDiscussionsAsync_WhenOnline_ReturnsDiscussions()
    {
        // Arrange
        var courseId = 1;
        var expectedDiscussions = new List<Discussion>
        {
            new() { Id = 1, CourseId = courseId, Title = "Test Discussion" }
        };

        _httpClientMock.Setup(c => c.GetFromJsonAsync<List<Discussion>>($"api/discussions/{courseId}"))
            .ReturnsAsync(expectedDiscussions);

        // Act
        var result = await _service.GetDiscussionsAsync(courseId);

        // Assert
        Assert.Equal(expectedDiscussions.Count, result.Count);
        Assert.Equal(expectedDiscussions[0].Title, result[0].Title);
    }
}