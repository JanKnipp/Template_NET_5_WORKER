using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Template_NET_CORE_3_WORKER.CoreService.HostedServices;
using Template_NET_CORE_3_WORKER.CoreService.Models;

namespace Template_NET_CORE_3_WORKER.CoreService
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        private static IConfigurationRoot GetConfiguration()
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            return configBuilder.Build();
        }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                _configuration = GetConfiguration();

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();

                Log.Information("Host {State} {DateTime}", LifeTimeState.Starting, DateTimeOffset.Now);
                await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            }
            finally
            {
                Log.Information("Host {State} {DateTime}", LifeTimeState.Stopping, DateTimeOffset.Now);
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .ConfigureHostConfiguration(c => { c.AddConfiguration(_configuration); })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(
                    (context, builder) =>
                    {
                        builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

                        builder.RegisterType<LifeTimeEventService>().As<IHostedService>();
                        builder.RegisterType<SampleQuartzService>().As<IHostedService>();

                        builder.RegisterLogger();
                    });
    }
}