namespace Template_NET_5_WORKER.CoreService.Configuration
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Quartz;

    using Template_NET_5_WORKER.CoreService.Models.Configuration;
    using Template_NET_5_WORKER.CoreService.QuartzJobs;

    internal static class CustomQuartz
    {
        internal static IServiceCollection AddCustomQuartz(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddQuartz(
                configurator =>
                    {
                        configurator.UseMicrosoftDependencyInjectionJobFactory();

                        configurator.AddJobAndTrigger<SampleJob>(configuration);
                    });

            collection.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            return collection;
        }

        private static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            IConfiguration config)
            where T : IJob
        {
            const string QuartzConfig = "Quartz";
            const string GroupName = "Template_NET_5_Worker";

            var jobName = typeof(T).Name;

            var quartzJobConfiguration = config.GetSection(QuartzConfig)?.GetSection(jobName)?.Get<QuartzJobConfiguration>();

            if (string.IsNullOrEmpty(quartzJobConfiguration?.CronConfig))
            {
                Serilog.Log.Warning("CronConfig for {JobName} not configured, Job will not be configured!", jobName);
                return;
            }

            if (!CronExpression.IsValidExpression(quartzJobConfiguration?.CronConfig))
            {
                Serilog.Log.Warning("CronConfig for {JobName} is invalid, Job will not be configured!", jobName);
                return;
            }

            var jobKey = new JobKey(jobName, GroupName);

            quartz.AddJob<T>(configurator => configurator.WithIdentity(jobKey));
            quartz.AddTrigger(
                configurator => configurator.ForJob(jobKey)
                    .WithCronSchedule(quartzJobConfiguration?.CronConfig)
                    .WithIdentity($"{jobName}Trigger"));
        }
    }
}