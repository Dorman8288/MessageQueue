using MessageQueue.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private static TopicManager _manager;
        private static ILogger<TopicController> _logger;

        public TopicController(ILogger<TopicController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateTopic([FromBody] TopicCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) || TimeSpan.FromMinutes(request.RetensionInMinuets) <= TimeSpan.Zero)
                {
                    return BadRequest("Invalid topic data.");
                }

                if (TopicManager.TopicExists(request.Name))
                    return BadRequest("This Topic Already Exists");
                var newTopic = new Topic(request.Name, TimeSpan.FromMinutes(request.RetensionInMinuets), _logger);
                newTopic.LoadMessages();
                TopicManager.Add(newTopic);

                return CreatedAtAction(nameof(GetTopics), new { name = newTopic.Name }, newTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topic.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: api/topics
        [HttpGet]
        public IActionResult GetTopics()
        {
            try
            {
                var topicNames = new List<string>();
                foreach (var topic in TopicManager.GetTopics())
                {
                    topicNames.Add(topic.Name);
                }

                return Ok(topicNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topics.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }

    public class TopicCreateRequest
    {
        public string Name { get; set; }
        public int RetensionInMinuets { get; set; }
    }
}
