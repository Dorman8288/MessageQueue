using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MessageQueue.Models
{
    public class TopicManager
    {
        private static List<Topic> topics;
        private static object _lock = new object();
        private static string TopicRepoPath;
        private static ILogger _logger;

        static TopicManager()
        {
            _logger = NullLogger<string>.Instance;
            TopicRepoPath = @".\Data\Topics.json";
            LoadTopics();
        }
        public TopicManager(ILogger logger, string topicRepoPath) 
        {
            _logger = logger;
            TopicRepoPath = topicRepoPath;
        }

        public static void CleanUp()
        {
            lock (_lock)
            {
                var topics = GetTopics();
                foreach (var topic in topics)
                    topic.RemoveExpiredMessages();
            }
        }

        public static void Add(Topic topic)
        {
            lock (_lock)
            {
                _logger.LogInformation($"Registering Topic {topic.Name}");
                topics.Add(topic);
                UpdateTopicRepo();
                _logger.LogInformation($"Successfully Registered Topic {topic.Name}");
            }
        }

        public static void Remove(Topic topic)
        {
            lock (_lock)
            {
                _logger.LogInformation($"Registering Topic {topic.Name}");
                topics.Remove(topic);
                UpdateTopicRepo();
                _logger.LogInformation($"Successfully Registered Topic {topic.Name}");
            }
        }

        public static Topic GetTopic(string name)
        {
            lock (_lock)
            {
                var topic = topics.FirstOrDefault(x => x.Name == name);
                topic.LoadMessages();
                return topic;
            }
        }
        public static IEnumerable<Topic> GetTopics() => topics;
        public static bool TopicExists(string name)
        {
            lock (_lock)
            {
                return topics.Any(x => x.Name == name);
            }
        }


        public static void UpdateTopicRepo()
        {
            lock (_lock)
            {
                _logger.LogInformation("Updating Topic Repo");
                string serializedTopics = JsonConvert.SerializeObject(topics);
                File.WriteAllText(TopicRepoPath, serializedTopics);
                _logger.LogInformation("Successfully Updated Topic Repo");
            }
        }

        public static void LoadTopics()
        {
            if (File.Exists(TopicRepoPath))
            {
                _logger.LogInformation($"Found Topic Repository at ({TopicRepoPath}).");
                _logger.LogInformation($"Topic Loading Starting.");
                string serializedTopics = File.ReadAllText(TopicRepoPath);
                List<Topic> extractedTopics;
                extractedTopics = JsonConvert.DeserializeObject<List<Topic>>(serializedTopics);
                if (extractedTopics != null)
                {
                    _logger.LogInformation("Extracted the following Topics");
                    var sequenceNumber = int.MinValue;
                    foreach (var topic in extractedTopics)
                        if (topic is Topic T)
                            _logger.LogInformation($"Topic {T.Name}");
                    topics = extractedTopics;
                }
                else
                {
                    _logger.LogWarning("No Topics have been extracted.");
                    topics = [];
                }
            }
            else
            {
                _logger.LogWarning($"The Topic Repository Does Not Exists. Creating a new Topic Repository at ({TopicRepoPath})");
                File.Create(TopicRepoPath).Close();
                topics = [];
            }
        }
    }
}
