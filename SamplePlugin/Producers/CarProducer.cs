using Common;
using Common.Attributes;
using Common.Interfaces;
using SamplePlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin.Producers
{
    [ProducerConsumerImplementationData(3, 1)]
    internal class CarProducer : IProducer<Car>
    {
        public CarProducer() { }
        public Car Produce()
        {
            Thread.Sleep(5000);
            var random = new Random();
            var generate = Utils.GenerateRandomString;
            var maxStringLength = 100;
            var RandomCar = new Car(generate(random.Next(100)),
                generate(random.Next(maxStringLength)),
                random.Next(),
                generate(random.Next(maxStringLength)),
                random.NextDouble());
            return RandomCar;
        }
    }
}
