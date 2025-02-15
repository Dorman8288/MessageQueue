using MessageQueue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessageQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishedEventController : ControllerBase
    {
        private readonly ILogger<PublishedEventController> _logger;

        public PublishedEventController(ILogger<PublishedEventController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public ActionResult<PublishedEvent> Post([FromBody] PublishedEvent Event)
        {
            if (Event == null)
            {
                return BadRequest("Invalid product.");
            }
            string TopicRepoPath = @".\Data\Topics.json";
            Topic topic;
            if (!TopicManager.TopicExists(Event.TopicName))
            {
                topic = new Topic(Event.TopicName, default, _logger);
                TopicManager.Add(topic);
            }
            else
                topic = TopicManager.GetTopic(Event.TopicName);

            topic.PublishMessage(Event.Value);
            return CreatedAtAction(nameof(Post), new { TopicName = Event.TopicName, PublishedMessage = Event.Value }, Event);
        }
    }
}
