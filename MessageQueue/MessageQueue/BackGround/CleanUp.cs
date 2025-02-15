using MessageQueue.Models;
using Microsoft.Extensions.Hosting;

namespace MessageQueue.BackGround
{
    public class CleanUpService : BackgroundService
    {
        private readonly ILogger<CleanUpService> _logger;

        public CleanUpService(ILogger<CleanUpService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cleanup service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background task is running at {time}", DateTimeOffset.Now);
                TopicManager.CleanUp();
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }

            _logger.LogInformation("Cleanup service stopping.");
        }

    }
}
