namespace Template_NET_CORE_3_WORKER.CoreService.QuartzJobs
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.Extensions.Logging;

    using Quartz;

    using Template_NET_CORE_3_WORKER.Contracts;

    [DisallowConcurrentExecution]
    internal class SampleJob : IJob
    {
        private readonly ILogger<SampleJob> _log;

        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly IPublishEndpoint _publishEndpoint;

        private readonly string _jobName;

        public SampleJob(ILogger<SampleJob> log, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._sendEndpointProvider = sendEndpointProvider;
            this._publishEndpoint = publishEndpoint;

            this._jobName = this.GetType().Name;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this._log.LogDebug("Scheduler executed {JobName} at {SchedulerFireTimeUtc}, next execution will be at {SchedulerNextFireTimeUtc}", this._jobName, context.FireTimeUtc, context.NextFireTimeUtc);

            await this._publishEndpoint.Publish<IOfferRequest>(
                new { Text = "test", TimeStamp = DateTime.UtcNow, OfferId = Guid.NewGuid(), Number = 1 },
                context.CancellationToken).ConfigureAwait(false);
        }
    }
}