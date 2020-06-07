namespace Template_NET_CORE_3_WORKER.CoreService
{
    using System;
    using System.Reflection;
    using System.Security.Authentication;

    using GreenPipes;

    using MassTransit;
    using MassTransit.Context;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;

    internal static class CustomMassTransit
    {
        internal static IServiceCollection AddCustomMassTransit(this IServiceCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var serviceProvider = collection.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            collection.TryAddSingleton(CustomEndpointNameFormatter.Instance);

            collection.AddSingleton<MassTransitRabbitConfig>(
                x =>
                    {
                        var config = x.GetService<IConfiguration>();

                        return config.GetSection(nameof(MassTransitRabbitConfig)).Get<MassTransitRabbitConfig>()
                               ?? new MassTransitRabbitConfig();
                    });

            collection.AddMassTransit(
                configurator =>
                    {
                        configurator.AddConsumers(Assembly.GetExecutingAssembly());

                        configurator.AddBus(AddCustomBus);
                    });
            collection.AddMassTransitHostedService();

            

            return collection;
        }

        private static IBusControl AddCustomBus(IRegistrationContext<IServiceProvider> registrationContext)
        {
            var massTransitRabbitConfig = registrationContext.Container.GetService<MassTransitRabbitConfig>();

            var busControl = Bus.Factory.CreateUsingRabbitMq(
                configurator =>
                    {
                        configurator.UseHealthCheck(registrationContext);

                        var clusterInternalName = massTransitRabbitConfig.ClusterName;
                        var vHost = massTransitRabbitConfig.VirtualHost;
                        var connectionName =
                            $"{Assembly.GetEntryAssembly()?.GetName().Name} ({Environment.MachineName})";
                        var host = configurator.Host(
                            new UriBuilder("rabbitmq", clusterInternalName, massTransitRabbitConfig.ClusterPort, vHost)
                                .Uri,
                            connectionName,
                            hostConfigurator =>
                                {
                                    hostConfigurator.Username(massTransitRabbitConfig.UserName);
                                    hostConfigurator.Password(massTransitRabbitConfig.Password);
                                    hostConfigurator.PublisherConfirmation = true;
                                    hostConfigurator.UseCluster(
                                        cluster => { massTransitRabbitConfig.ClusterNodes.ForEach(cluster.Node); });
                                    if (massTransitRabbitConfig.UseSSL)
                                    {
                                        hostConfigurator.UseSsl(sslConfigurator => sslConfigurator.Protocol = SslProtocols.None);
                                    }
                                });
                        configurator.UseRetry(
                            retryConfig => retryConfig.Exponential(
                                5,
                                TimeSpan.FromMilliseconds(200),
                                TimeSpan.FromSeconds(60),
                                TimeSpan.FromSeconds(1)));
                        configurator.ConfigureEndpoints(registrationContext);
                    });
            return busControl;
        }
    }
}