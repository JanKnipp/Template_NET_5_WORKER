namespace Template_NET_5_WORKER.CoreService.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Template_NET_5_WORKER.CoreService.Models;

    internal class LifeTimeEventService : IHostedService
    {
        private readonly ILogger<LifeTimeEventService> _log;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly string _serviceName;

        public LifeTimeEventService(ILogger<LifeTimeEventService> log, IHostApplicationLifetime hostApplicationLifetime)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));

            this._serviceName = this.GetType().Name;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this._log.LogInformation("{State} service {ServiceName}", LifeTimeState.Starting, this._serviceName);

            // setup lifetime events
            this._hostApplicationLifetime.ApplicationStarted.Register(this.OnApplicationStarted);
            this._hostApplicationLifetime.ApplicationStopping.Register(this.OnApplicationStopping);
            this._hostApplicationLifetime.ApplicationStopped.Register(this.OnApplicationStopped);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._log.LogInformation("{State} service {ServiceName}", LifeTimeState.Stopping, this._serviceName);
        }

        private void OnApplicationStarted()
        {
            this._log.LogDebug("OnApplicationStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnApplicationStopping()
        {
            this._log.LogDebug("OnApplicationStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnApplicationStopped()
        {
            this._log.LogDebug("OnApplicationStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}