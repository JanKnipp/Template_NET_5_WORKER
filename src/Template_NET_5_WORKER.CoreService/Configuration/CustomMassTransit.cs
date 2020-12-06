namespace Template_NET_5_WORKER.CoreService.Configuration
{
    using System;
    using System.Reflection;
    using System.Security.Authentication;

    using GreenPipes;

    using MassTransit;
    using MassTransit.Conductor;
    using MassTransit.Context;
    using MassTransit.Definition;
    using MassTransit.RabbitMqTransport;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    using Template_NET_5_WORKER.Components;
    using Template_NET_5_WORKER.Components.MassTransit.Activities;
    using Template_NET_5_WORKER.Components.MassTransit.StateMachines;
    using Template_NET_5_WORKER.CoreService.Models.Configuration;

    internal static class CustomMassTransit
    {
        internal static IServiceCollection AddCustomMassTransit(this IServiceCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }


            collection.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            collection.TryAddScoped<RequestOfferActivity>();

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
                        configurator.AddRabbitMqMessageScheduler();

                        configurator.AddConsumersFromNamespaceContaining<IComponents>();
                        configurator.AddSagasFromNamespaceContaining<IComponents>();
                        configurator.AddSagaStateMachinesFromNamespaceContaining<IComponents>();
                        configurator.AddActivitiesFromNamespaceContaining<IComponents>();

                        configurator
                            .AddSagaStateMachine<OfferStateMachine, OfferState>(typeof(OfferStateMachineDefinition))
                            .MongoDbRepository(
                                repositoryConfigurator =>
                                    {
                                        repositoryConfigurator.Connection = "mongodb://mongo";
                                        repositoryConfigurator.DatabaseName = "offer";
                                    });

                        configurator.AddServiceClient();

                        configurator.UsingRabbitMq(ConfigureRabbitMq);
                    });

            collection.AddMassTransitHostedService();

            return collection;
        }

        public static IApplicationBuilder UseCustomMassTransit(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<LoggerFactory>();

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            return app;
        }

        private static void ConfigureRabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            var massTransitRabbitConfig = context.GetService<MassTransitRabbitConfig>();

            configurator.UseHealthCheck(context);
           
            var clusterInternalName = massTransitRabbitConfig.ClusterName;
            var virtualHost = massTransitRabbitConfig.VirtualHost;
            var connectionName =
                $"{Assembly.GetEntryAssembly()?.GetName().Name} ({Environment.MachineName})";
            configurator.Host(
                new UriBuilder("rabbitmq", clusterInternalName, massTransitRabbitConfig.ClusterPort, virtualHost)
                    .Uri,
                connectionName,
                hostConfigurator =>
                    {
                        hostConfigurator.Username(massTransitRabbitConfig.UserName);
                        hostConfigurator.Password(massTransitRabbitConfig.Password);
                        hostConfigurator.PublisherConfirmation = true;
                        hostConfigurator.UseCluster(
                            cluster =>
                                {
                                    
                                    foreach (var node in massTransitRabbitConfig.ClusterNodes)
                                    {
                                        cluster.Node(node);
                                    }
                                });
                        if (massTransitRabbitConfig.UseSSL)
                        {
                            hostConfigurator.UseSsl(sslConfigurator => sslConfigurator.Protocol = SslProtocols.None);
                        }
                    });

            configurator.UseRabbitMqMessageScheduler();
            
            configurator.UseRetry(
                retryConfig => retryConfig.Exponential(
                    5,
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(1)));

            var serviceInstanceOptions = new ServiceInstanceOptions()
                .EnableInstanceEndpoint()
                .EnableJobServiceEndpoints()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ConfigureServiceEndpoints(context, serviceInstanceOptions);

            // configurator.ConfigureEndpoints(context);
        }
    }
}