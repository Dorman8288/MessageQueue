using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Interfaces;
using SamplePlugin.Models;
using Common.Attributes;

namespace SamplePlugin.Consumers
{
    [ProducerConsumerImplementationData(2, 2)]
    internal class CarConsumer : IConsumer<Car>
    {
        public CarConsumer() { }
        public void Consume(Car item)
        {
            Thread.Sleep(5000);
        }
    }
}
