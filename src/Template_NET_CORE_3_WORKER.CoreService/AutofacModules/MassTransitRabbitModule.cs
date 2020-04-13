using System;
using System.Reflection;
using System.Security.Authentication;
using Autofac;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template_NET_CORE_3_WORKER.CoreService.MassTransitConsumers;
using Template_NET_CORE_3_WORKER.CoreService.Models;
using Template_NET_CORE_3_WORKER.CoreService.Models.Configuration;
using Module = Autofac.Module;

namespace Template_NET_CORE_3_WORKER.CoreService.AutofacModules
{
    public class MassTransitRabbitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var config = c.Resolve<IConfiguration>();
                    return config.GetSection(typeof(MassTransitRabbitConfig).Name).Get<MassTransitRabbitConfig>();
                }).As<MassTransitRabbitConfig>().SingleInstance();

            builder.Register(
                c =>
                {
                    var rabbitConfig = c.Resolve<MassTransitRabbitConfig>();

                    LogContext.ConfigureCurrentLogContext(c.Resolve<ILoggerFactory>());

                    var bus = Bus.Factory.CreateUsingRabbitMq(
                        configurator =>
                        {
                            var clusterInternalName = rabbitConfig.ClusterName;
                            var vHost = rabbitConfig.VirtualHost;
                            var connectionName =
                                    $"{Assembly.GetEntryAssembly()?.GetName().Name} ({Environment.MachineName})";
                            var host = configurator.Host(
                                    new UriBuilder("rabbitmq", clusterInternalName, rabbitConfig.ClusterPort, vHost).Uri,
                                    connectionName,
                                    hostConfigurator =>
                                    {
                                        hostConfigurator.Username(rabbitConfig.UserName);
                                        hostConfigurator.Password(rabbitConfig.Password);
                                        hostConfigurator.PublisherConfirmation = true;
                                        hostConfigurator.UseCluster(
                                                cluster =>
                                                {
                                                    rabbitConfig.ClusterNodes.ForEach(cluster.Node);
                                                });
                                        hostConfigurator.UseSsl(sslConfigurator => sslConfigurator.Protocol = SslProtocols.None);
                                    });

                            configurator.UseRetry(
                                    retryConfig => retryConfig.Exponential(
                                        5,
                                        TimeSpan.FromMilliseconds(200),
                                        TimeSpan.FromSeconds(60),
                                        TimeSpan.FromSeconds(1)));

                            configurator.ReceiveEndpoint(
                                    $"{typeof(SampleRequest).FullName}::{AppDomain.CurrentDomain.FriendlyName}",
                                    endpointConfigurator =>
                                    {
                                        endpointConfigurator.PrefetchCount = 1;
                                        endpointConfigurator.Consumer<SampleConsumer>(c);
                                    });
                        });
                    return bus;
                }).SingleInstance().As<IBusControl>().As<IBus>();

            builder.RegisterConsumers(Assembly.GetExecutingAssembly());
        }
    }
}