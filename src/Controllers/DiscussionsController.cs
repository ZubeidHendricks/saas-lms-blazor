[ApiController]
[Route("api/[controller]")]
public class DiscussionsController : ControllerBase
{
    private readonly IDiscussionService _discussionService;
    private readonly ILogger<DiscussionsController> _logger;

    public DiscussionsController(IDiscussionService discussionService, ILogger<DiscussionsController> logger)
    {
        _discussionService = discussionService;
        _logger = logger;
    }

    [HttpGet("{courseId}")]
    public async Task<ActionResult<List<Discussion>>> GetDiscussions(int courseId)
    {
        try
        {
            var discussions = await _discussionService.GetDiscussionsAsync(courseId);
            return Ok(discussions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving discussions for course {CourseId}", courseId);
            return StatusCode(500, "An error occurred while retrieving discussions");
        }
    }
}