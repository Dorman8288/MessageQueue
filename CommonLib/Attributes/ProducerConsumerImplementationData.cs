using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProducerConsumerImplementationData : Attribute
    {
        public int retryNumber;
        public int rateLimit;
        public string? topicName;
        public ProducerConsumerImplementationData(int retryNumber = 3, int rateLimit = 3, string? topicName = null)
        {
            this.retryNumber = retryNumber;
            this.rateLimit = rateLimit; 
            this.topicName = topicName;
        }
    }
}
