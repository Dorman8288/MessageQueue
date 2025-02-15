using Common.Interfaces;
using Common.Loggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class DataPublisher
    {
        string uri;
        int retryCount;
        ILogger logger;
        public bool InfiniteMode;
        public DataPublisher(string uri, int retryCount, ILogger logger, bool infiniteMode = false)
        {
            this.logger = logger;
            this.uri = uri;
            this.retryCount = retryCount;
            this.InfiniteMode = infiniteMode;
        }

        public async Task<bool> Publish(string topicName, string value)
        {
            while (retryCount > 0 || InfiniteMode)
            {
                if(await TryPublish(topicName, value))
                    return true;
                retryCount--;
            }
            return false;
        }

        public async Task<bool> TryPublish(string topicName, string value)
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(uri);
            var payload = new
            {
                topicName,
                value
            };
            var jsonContent = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            logger.Info($"Attempting to publish {jsonContent} on {topicName}");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            var responsePromise = client.PostAsync("/api/PublishedEvent", content, token);
            if (await Task.WhenAny(responsePromise, Task.Delay(5000, token)) == responsePromise)
            {
                var response = responsePromise.Result;
                if (response.IsSuccessStatusCode)
                {
                    logger.Info($"Successfully publish {jsonContent} on {topicName}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    logger.Info($"Response: {responseBody}");
                    return true;
                }
                else
                {
                    logger.Error($"Publishing {jsonContent} on {topicName} failed with status code: {response.StatusCode}" + (InfiniteMode ? "" : $"Retries Left {retryCount}"));
                    return false;
                }
            }
            else
            {
                logger.Error($"Connection Timed Out" + (InfiniteMode ? "" : $"Retries Left {retryCount}"));
                return false;
            }
        }
    }
}
