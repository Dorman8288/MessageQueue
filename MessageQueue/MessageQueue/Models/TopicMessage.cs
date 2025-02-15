using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace MessageQueue.Models
{
    public class TopicMessage
    {
        [JsonIgnore]
        public int SequenceNumber { get; set; }
        public string TopicName { get; set; }
        public DateTime ExpirationDate { get; set; }
        
        public bool IsExpired
        {
            get { return ExpirationDate < DateTime.Now; }
        }
        public string Message { get; set; }


        public TopicMessage() { }

        public TopicMessage(int sequenceNumber, string topicName, DateTime expirationDate, string message)
        {
            SequenceNumber = sequenceNumber;
            TopicName = topicName;
            ExpirationDate = expirationDate;
            Message = message;
        }
    }
}
