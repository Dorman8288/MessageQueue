using Common.Attributes;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin.Producers
{
    [ProducerConsumerImplementationData(3, 2)]
    public class RandomIntProducer : IProducer<int>
    {
        public RandomIntProducer() { }
        public int Produce()
        {
            Thread.Sleep(2000);
            Random random = new Random();
            return random.Next();
        }
    }
}
