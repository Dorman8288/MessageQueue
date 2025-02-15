using MessageQueue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpGet("{topicName}/{sequenceNumber}")]
        public ActionResult<TopicMessage> Get(string topicName, int sequenceNumber)
        {
            if (!TopicManager.TopicExists(topicName))
                return NotFound($"Could Not Found Topic {topicName}");
            var topic = TopicManager.GetTopic(topicName);
            if (topic.currentSequenceNumber == sequenceNumber)
                return NotFound(new
                {
                    Error = $"No new Messages in Topic {topicName}"
                });


            var message = topic.GetMessage(sequenceNumber);
            if (message == null)
                return NotFound(new {
                    Error = $"Could Not Find Message Number {sequenceNumber} in topic {topicName}",
                    LatestSequenceNumber = topic.GetOldestSequenceNumber()
                });
            return Ok(message);
        }
    }
}
