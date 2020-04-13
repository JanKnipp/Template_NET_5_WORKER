using Autofac;
using Autofac.Extras.Quartz;
using Microsoft.Extensions.Configuration;
using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;

namespace Template_NET_CORE_3_WORKER.CoreService.AutofacModules
{
    internal class QuartzJobModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var config = c.Resolve<IConfiguration>();
                    return config.GetSection(typeof(SampleQuartzServiceOptions).Name).Get<SampleQuartzServiceOptions>() ?? new SampleQuartzServiceOptions();
                }).As<SampleQuartzServiceOptions>().SingleInstance();

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(ThisAssembly));
        }
    }
}