namespace Template_NET_5_WORKER.CoreService.Configuration
{
    using System;
    using System.Reflection;

    using Jaeger;
    using Jaeger.Samplers;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using OpenTracing;
    using OpenTracing.Util;

    public static class CustomJaeger
    {

        public static IServiceCollection AddCustomJaeger(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ITracer>(serviceProvider =>
                {
                    var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

                    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                    var sampler = new ConstSampler(sample: true);

                    var tracer = new Tracer.Builder(serviceName)
                        .WithLoggerFactory(loggerFactory)
                        .WithSampler(sampler)
                        .Build();

                    GlobalTracer.Register(tracer);

                    return tracer;
                });

            return services;
        }
    }
}
