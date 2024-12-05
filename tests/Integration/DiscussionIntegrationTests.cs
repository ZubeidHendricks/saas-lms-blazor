public class DiscussionIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DiscussionIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateDiscussion_ReturnsCreatedDiscussion()
    {
        // Arrange
        var discussion = new Discussion
        {
            CourseId = 1,
            Title = "Integration Test Discussion",
            Content = "Test Content",
            AuthorId = "test-user"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/discussions", discussion);
        var createdDiscussion = await response.Content.ReadFromJsonAsync<Discussion>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdDiscussion);
        Assert.Equal(discussion.Title, createdDiscussion.Title);
    }
}