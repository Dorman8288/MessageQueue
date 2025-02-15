using CommonLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;

namespace SamplePlugin.Consumers
{
    [ProducerConsumerImplementationData(1, 1)]
    internal class RandomStringConsumer : IConsumer<string>
    {
        public void Consume(string item)
        {
            Thread.Sleep(3000);
        }
    }
}
