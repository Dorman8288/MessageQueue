using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace MessageQueue.Models
{
    public class Topic
    {
        const string TopicRepoPath = @".\Data\Topics.json";
        public string Name { get; set; }
        public string FolderPath;
        public TimeSpan retension;
        public int currentSequenceNumber;
        [JsonIgnore]
        private List<TopicMessage> topicMessages;
        private object messageLock;
        static ILogger _logger { get; set; }

        public Topic(string name, TimeSpan retension = default, ILogger logger = null)
        {
            _logger = logger;
            if (logger == null)
            {
                var loggerFactory = new LoggerFactory();
                _logger = loggerFactory.CreateLogger("Console");
            }
            _logger.LogInformation($"Creating New Topic {name}");
            Name = name;
            FolderPath = @$".\Data\{Name}\";
            if (retension == default)
                this.retension = TimeSpan.FromSeconds(60);
            else
                this.retension = retension;
            messageLock = new object();
            topicMessages = new List<TopicMessage>();
            LoadMessages();
        }

        public void LoadMessages()
        {
            lock (messageLock)
            {
                topicMessages = new List<TopicMessage>();
            }
            if (Directory.Exists(FolderPath))
            {
                var files = Directory.GetFiles(FolderPath);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".json")
                    {
                        try
                        {
                            TopicMessage message;
                            lock (messageLock)
                            {
                                var fileSequenceNumber = int.Parse(Path.GetFileNameWithoutExtension(file));
                                string data = File.ReadAllText(file);
                                message = JsonConvert.DeserializeObject<TopicMessage>(data);
                            }
                            if(message != null)
                                PublishMessage(message);
                        }catch(Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning($"Topics Message Folder Does not exists in {FolderPath}. creating a new Message Folder");
                Directory.CreateDirectory(FolderPath);
            }
        }

        public TopicMessage? GetMessage(int sequenceNumber)
        {
            lock (messageLock)
            {
                return topicMessages.FirstOrDefault(x => x.SequenceNumber == sequenceNumber);
            }
        }

        public bool PublishMessage(TopicMessage message)
        {
            if (!message.IsExpired)
            {
                lock (messageLock)
                {
                    topicMessages.Add(message);
                    string serializedMessage = JsonConvert.SerializeObject(message);
                    File.WriteAllText($"{FolderPath}{message.SequenceNumber}.json", serializedMessage);
                    return true;
                }
            }
            return false;
        }

        public void RemoveMessage(TopicMessage message)
        {
            lock (messageLock)
            {
                topicMessages.Remove(message);
                string path = $"{FolderPath}{message.SequenceNumber}.json";
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        public int GetOldestSequenceNumber()
        {
            lock (messageLock)
            {
                return topicMessages.Count == 0 ? 0 : topicMessages.Min(x => x.SequenceNumber);
            }
        }

        public void RemoveExpiredMessages()
        {
            lock (messageLock)
            {
                var expired = topicMessages.Where(x => x.IsExpired);
                foreach (var message in expired)
                    RemoveMessage(message);
            }
        }

        public bool PublishMessage(string value)
        {
            var result = PublishMessage(new TopicMessage(currentSequenceNumber++, Name, DateTime.Now + retension, value));
            if (result)
                TopicManager.UpdateTopicRepo();
            return result;
        }   
    }
}
