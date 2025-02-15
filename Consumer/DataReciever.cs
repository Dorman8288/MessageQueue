using Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    internal class DataReciever
    {
        string url;
        int retryCount;
        ILogger logger;
        int sequenceNumber;
        public bool InfiniteMode;

        public DataReciever(string url, ILogger logger, int sequenceNumber = 0, int retryNumber = 3, bool infiniteMode = false)
        {
            this.url = url;
            this.retryCount = retryNumber;
            this.logger = logger;
            this.sequenceNumber = sequenceNumber;
            this.InfiniteMode = infiniteMode;
        }

        public async Task<string> Recieve(string topicName)
        {
            while (retryCount > 0 || InfiniteMode)
            {
                try
                {
                    var messagePromise = await TryRecieve(topicName, sequenceNumber);
                    if (messagePromise != null)
                    {
                        var responseTemplate = new { error = "", latestSequenceNumber = 0 };
                        var response = JsonConvert.DeserializeAnonymousType(messagePromise, responseTemplate);
                        if (response.error is not null)
                        {
                            if(response.latestSequenceNumber > sequenceNumber)
                            {
                                logger.Warn($"The Topic Only Has Messages From after {response.latestSequenceNumber}. this is likely due to them being Expired");
                                logger.Info($"setting the active sequece number to {response.latestSequenceNumber}");
                                sequenceNumber = response.latestSequenceNumber;
                            }
                        }
                        else
                            sequenceNumber++;

                        return messagePromise;
                    }
                    retryCount--;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    retryCount--;
                }
            }
            logger.Warn("Max Retry Count Reached Shutting Down Reciever");
            return null;
        }

        public async Task<string> TryRecieve(string topicName, int sequenceNumber)
        {
            using HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(url);
            logger.Info($"Attempting to Receive Message {sequenceNumber} from Topic {topicName}");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            var responsePromise = client.GetAsync($"/api/Message/{topicName}/{sequenceNumber}", token);
            if (await Task.WhenAny(responsePromise, Task.Delay(5000, token)) == responsePromise)
            {
                var response = responsePromise.Result;
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    logger.Info($"Successfully Received {message} from {topicName}");
                    return message;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    logger.Warn($"Request failed with status code: {response.StatusCode}" + (InfiniteMode ? "" : $"number of Retries Left {retryCount}"));
                    logger.Warn(message);
                    return message;
                }
            }
            else
            {
                logger.Error("Connection Timed Out" + (InfiniteMode ? "" : $"number of Retries Left {retryCount}"));
                return null;
            }
        }
    }
}
