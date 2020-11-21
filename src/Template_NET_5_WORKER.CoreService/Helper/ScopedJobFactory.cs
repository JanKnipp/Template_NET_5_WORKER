namespace Template_NET_5_WORKER.CoreService.Helper
{
    using System;
    using System.Collections.Concurrent;

    using Microsoft.Extensions.DependencyInjection;

    using Quartz;
    using Quartz.Spi;

    public class ScopedJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<IJob, IServiceScope> _serviceScopes = new ConcurrentDictionary<IJob, IServiceScope>();

        public ScopedJobFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;
            var scope = this._serviceProvider.CreateScope();

            var job = (IJob)(scope.ServiceProvider.GetService(jobType)
                             ?? ActivatorUtilities.CreateInstance(scope.ServiceProvider, jobType));

            this._serviceScopes.TryAdd(job, scope);
            return job;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();

            if (this._serviceScopes.TryRemove(job, out var scope))
            {
                scope.Dispose();
            }
        }
    }
}
