using Common.Attributes;
using Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using CommonLib.Interfaces;

namespace Consumer
{
    internal class ConsumptionUnit
    {
        private IEnumerable<Type> consumers;
        ILogger logger;
        string url;
        public ConsumptionUnit(IEnumerable<Type> consumers, string url, ILogger logger)
        {
            this.consumers = consumers;
            this.url = url;
            this.logger = logger;
        }


        public void Worker(ProducerConsumerImplementationData args, Type type, string message)
        {
            while (true)
            {
                //try
                //{
                    var ProductType = type.GetInterface(typeof(IConsumer<>).Name).GetGenericArguments()[0];
                    var consumer = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("Consume");
                    var product = JsonConvert.DeserializeObject(message, ProductType);
                    args.topicName ??= product.GetType().Name;
                    var _ = methodInfo?.Invoke(consumer, [product]);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
        }
        



        public async Task StartConsumption(Type type)
        {
            var args = (ProducerConsumerImplementationData)type.GetCustomAttributes(typeof(ProducerConsumerImplementationData), false).FirstOrDefault();
            args ??= new ProducerConsumerImplementationData();
            args.topicName ??= type.GetInterface(typeof(IConsumer<>).Name).GetGenericArguments()[0].Name;
            var reciever = new DataReciever(url, logger) { InfiniteMode = true };
            int windowSize = args.rateLimit;
            ConcurrentBag<Task> window = new ConcurrentBag<Task>();
            logger.Info($"Starting {args.topicName} Dispatcher");
            while (true)
            {
                var message = await reciever.Recieve(args.topicName);
                if(message is null)
                    { break; }
                while (window.Count == windowSize)
                {
                    foreach(var worker in window)
                    {
                        if (worker.IsCompleted)
                        {
                            var result = worker;
                            window.TryTake(out result);
                        }
                    }
                }
                var task = Task.Run(async () =>
                {
                    await Task.Yield();
                    Worker(args, type, message);
                });
                window.Add(task);
            }
        }

        public async Task StartConsumption()
        {
            logger.Info("Starting Production");
            List<Task> tasks = new List<Task>();
            foreach (var producer in consumers)
                tasks.Add(StartConsumption(producer));
            await Task.WhenAll(tasks);
        }
    }
}
