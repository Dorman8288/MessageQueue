using CommonLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;

namespace SamplePlugin.Consumers
{
    [ProducerConsumerImplementationData(1, 2)]
    internal class RandomIntConsumer : IConsumer<int>
    {
        public void Consume(int item)
        {
            Thread.Sleep(1000);
        }
    }
}
