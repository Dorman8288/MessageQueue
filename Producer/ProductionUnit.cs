using Common.Attributes;
using Common.Interfaces;
using CommonLib.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Producer
{
    internal class ProductionUnit
    {
        private IEnumerable<Type> producers;
        ILogger logger;
        string url;
        List<Task> tasks = new List<Task>();
        public ProductionUnit(IEnumerable<Type> producers, string url, ILogger logger)
        {
            this.producers = producers;
            this.url = url;
            this.logger = logger;
        }
        public void Worker(ProducerConsumerImplementationData args, Type type, DataPublisher publisher)
        {
            while (true)
            {
                try
                {
                    var producer = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("Produce");
                    var product = methodInfo?.Invoke(producer, null);
                    var serializedProduct = JsonConvert.SerializeObject(product);
                    args.topicName ??= type.GetInterface(typeof(IProducer<>).Name).GetGenericArguments()[0].Name;
                    var success = publisher.Publish(args.topicName, serializedProduct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public async Task StartProduction(Type type)
        {
            var args = (ProducerConsumerImplementationData)type.GetCustomAttributes(typeof(ProducerConsumerImplementationData), false).FirstOrDefault();
            args ??= new ProducerConsumerImplementationData();
            var publisher = new DataPublisher(url, args.retryNumber, logger, true);
            logger.Info($"Preparing {type} Producer");
            for (int i = 0; i < args.rateLimit; i++)
            {
                var task = Task.Run(async () =>
                {                    
                    Worker(args, type, publisher);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        public async Task StartProduction()
        {
            logger.Info("Starting Production");
            List<Task> tasks = new List<Task>();
            foreach (var producer in producers)
                tasks.Add(StartProduction(producer));
            await Task.WhenAll(tasks);
        }

    }
}
