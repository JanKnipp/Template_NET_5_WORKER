using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Template_NET_CORE_3_WORKER.CoreService.QuartzJobs
{
    [DisallowConcurrentExecution]
    internal class SampleJob : IJob
    {
        private readonly ILogger<SampleJob> _log;
        private readonly string _jobName;

        public SampleJob(ILogger<SampleJob> log)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._jobName = this.GetType().Name;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this._log.LogDebug("Scheduler executed {JobName} at {SchedulerFireTimeUtc}, next execution will be at {SchedulerNextFireTimeUtc}", this._jobName, context.FireTimeUtc, context.NextFireTimeUtc);
        }
    }
}