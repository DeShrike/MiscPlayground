namespace core.service
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly ILogger<TestService> _logger;

        private readonly IDemoService demoService;

        public TestService(ILogger<TestService> logger, IDemoService demoService)
        {
            this._logger = logger;
            this.demoService = demoService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogWarning("Starting");

            this._timer = new Timer(
                (e) => this.Ping(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(Program.AppSettings.PingInterval));

            return Task.CompletedTask;
        }

        public void Ping()
        {
            this._logger.LogInformation("Ping");
            this.demoService.DoSomething();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Stopping");

            this._timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }
    }
}
