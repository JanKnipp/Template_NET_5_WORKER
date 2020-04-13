using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Template_NET_CORE_3_WORKER.CoreService.Models;
using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;
using Template_NET_CORE_3_WORKER.CoreService.QuartzJobs;

namespace Template_NET_CORE_3_WORKER.CoreService.HostedServices
{
    internal class SampleQuartzService : IHostedService
    {
        private readonly ILogger<SampleQuartzService> _log;
        private readonly IScheduler _scheduler;
        private readonly SampleQuartzServiceOptions _sampleQuartzServiceOptions;
        private readonly string _serviceName;

        public SampleQuartzService(ILogger<SampleQuartzService> log, IScheduler scheduler,
            SampleQuartzServiceOptions sampleQuartzServiceOptions)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this._sampleQuartzServiceOptions = sampleQuartzServiceOptions ?? throw new ArgumentNullException(nameof(sampleQuartzServiceOptions));
            this._serviceName = this.GetType().Name;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("{ServiceName} is {State} with options {@ServiceOptions}",
                this._serviceName, LifeTimeState.Starting, this._sampleQuartzServiceOptions);

            var jobDetail = JobBuilder.Create<SampleJob>()
                .WithIdentity(nameof(SampleJob))
                .Build();
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule(_sampleQuartzServiceOptions.CronConfig,
                    builder => { builder.WithMisfireHandlingInstructionDoNothing(); })
                .WithIdentity($"{nameof(SampleJob)}Trigger")
                .StartNow()
                .Build();

            var initialStart = await _scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
            _log.LogInformation("{ServiceName} will start initially at {InitialStart}", this._serviceName, initialStart);
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("{ServiceName} is {State}", this._serviceName, LifeTimeState.Stopping);
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}