using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Template_NET_CORE_3_WORKER.CoreService.Models;

namespace Template_NET_CORE_3_WORKER.CoreService.HostedServices
{
    internal class LifeTimeEventService : IHostedService
    {
        private readonly ILogger<LifeTimeEventService> _log;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IBusControl _busControl;
        private readonly string _serviceName;

        public LifeTimeEventService(ILogger<LifeTimeEventService> log, IHostApplicationLifetime hostApplicationLifetime, IBusControl busControl)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            this._busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            this._serviceName = this.GetType().Name;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("{State} service {ServiceName}", LifeTimeState.Starting, this._serviceName);

            // setup lifetime events
            this._hostApplicationLifetime.ApplicationStarted.Register(this.OnApplicationStarted);
            this._hostApplicationLifetime.ApplicationStopping.Register(this.OnApplicationStopping);
            this._hostApplicationLifetime.ApplicationStopped.Register(this.OnApplicationStopped);

            // initialize MassTransit
            await this._busControl.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("{State} service {ServiceName}", LifeTimeState.Stopping, this._serviceName);

            await this._busControl.StopAsync(cancellationToken);
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