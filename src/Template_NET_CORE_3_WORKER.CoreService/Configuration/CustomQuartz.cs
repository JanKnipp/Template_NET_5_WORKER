namespace Template_NET_CORE_3_WORKER.CoreService.Configuration
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

    using Template_NET_CORE_3_WORKER.CoreService.Helper;
    using Template_NET_CORE_3_WORKER.CoreService.HostedServices;
    using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;

    internal static class CustomQuartz
    {
        internal static IServiceCollection AddCustomQuartz(this IServiceCollection collection)
        {
            collection.AddQuartz(
                configurator =>
                    {
                        configurator.UseMicrosoftDependencyInjectionJobFactory(
                            options => options.AllowDefaultConstructor = true);
                    });

            collection.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

            collection.AddSingleton<SampleQuartzServiceOptions>(
                x =>
                    {
                        var config = x.GetService<IConfiguration>();

                        return config.GetSection(nameof(SampleQuartzServiceOptions)).Get<SampleQuartzServiceOptions>()
                               ?? new SampleQuartzServiceOptions();
                    });

            collection.AddSingleton<ISchedulerFactory>(new StdSchedulerFactory());
            collection.AddSingleton<IJobFactory, ScopedJobFactory>();

            collection.AddHostedService<SampleQuartzService>();

            return collection;
        }
    }
}
