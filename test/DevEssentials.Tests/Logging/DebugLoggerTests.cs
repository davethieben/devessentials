using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Essentials.Test.Logging
{
    public class DebugLoggerTests
    {
        [Fact]
        public async Task CanLog()
        {
            var builder = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddBetterDebug(options =>
                    {
                    });
                    logging.AddConsole();
                    logging.AddDebug();

                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TestHostedService>();
                });

            using var host = builder.Build();
            await host.StartAsync();

            await host.StopAsync();

        }

        public class TestHostedService : BackgroundService
        {
            private readonly ILogger<TestHostedService> _logger;

            public TestHostedService(ILogger<TestHostedService> logger)
            {
                _logger = logger;
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogDebug("Debug Message");
                _logger.LogInformation("Info Message");
                _logger.LogError("Error Message");

                return Task.CompletedTask;
            }
        }
    }
}
