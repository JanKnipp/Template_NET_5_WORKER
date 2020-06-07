namespace Template_NET_CORE_3_WORKER.CoreService.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Quartz;
    using Quartz.Spi;

    using Template_NET_CORE_3_WORKER.CoreService.Models;
    using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;
    using Template_NET_CORE_3_WORKER.CoreService.QuartzJobs;

    internal class SampleQuartzService : IHostedService
    {
        private readonly ILogger<SampleQuartzService> _log;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly SampleQuartzServiceOptions _sampleQuartzServiceOptions;
        private readonly string _serviceName;
        private IScheduler _scheduler;

        public SampleQuartzService(ILogger<SampleQuartzService> log, ISchedulerFactory schedulerFactory, IJobFactory jobFactory, SampleQuartzServiceOptions sampleQuartzServiceOptions)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            this._jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            this._sampleQuartzServiceOptions = sampleQuartzServiceOptions ?? throw new ArgumentNullException(nameof(sampleQuartzServiceOptions));
            this._serviceName = this.GetType().Name;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this._scheduler = await this._schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
            this._scheduler.JobFactory = this._jobFactory;

            this._log.LogInformation(
                "{ServiceName} is {State} with options {@ServiceOptions}",
                this._serviceName,
                LifeTimeState.Starting,
                this._sampleQuartzServiceOptions);

            var jobDetail = JobBuilder.Create<SampleJob>()
                .WithIdentity(nameof(SampleJob))
                .Build();
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithCronSchedule(
                    this._sampleQuartzServiceOptions.CronConfig,
                    builder => { builder.WithMisfireHandlingInstructionDoNothing(); }).WithIdentity($"{nameof(SampleJob)}Trigger")
                .StartNow()
                .Build();

            var initialStart = await this._scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).ConfigureAwait(false);
            this._log.LogInformation("{ServiceName} will start initially at {InitialStart}", this._serviceName, initialStart);
            await this._scheduler.Start(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._log.LogInformation("{ServiceName} is {State}", this._serviceName, LifeTimeState.Stopping);
            await this._scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
        }
    }
}