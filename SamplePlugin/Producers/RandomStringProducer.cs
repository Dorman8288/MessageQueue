using Common;
using Common.Attributes;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin.Producers
{
    [ProducerConsumerImplementationData(3, 3)]
    public class RandomMessageProducer : IProducer<string>
    {
        public RandomMessageProducer()
        {
        }
        public string Produce()
        {
            Thread.Sleep(1000);
            return Utils.GenerateRandomString(32);
        }
    }
}
