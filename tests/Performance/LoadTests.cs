public class LoadTests
{
    private readonly HttpClient _client;
    private readonly ParallelOptions _parallelOptions;

    public LoadTests()
    {
        _client = new HttpClient();
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
    }

    [Fact]
    public async Task DiscussionApi_HandlesMultipleConcurrentRequests()
    {
        // Arrange
        const int numberOfRequests = 100;
        var stopwatch = new Stopwatch();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        stopwatch.Start();
        await Parallel.ForEachAsync(
            Enumerable.Range(1, numberOfRequests),
            _parallelOptions,
            async (i, ct) =>
            {
                var response = await _client.GetAsync($"/api/discussions/1");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should handle 100 requests in under 5 seconds
    }
}