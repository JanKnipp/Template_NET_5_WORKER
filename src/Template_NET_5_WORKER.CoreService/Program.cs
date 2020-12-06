namespace Template_NET_5_WORKER.CoreService
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    using Serilog;

    using Template_NET_5_WORKER.CoreService.Models;

    public static class Program
    {
        private static readonly string HostAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        public static async Task<int> Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

                Log.Information("Host {HostName} {State} {DateTime}", HostAssemblyName, LifeTimeState.Starting, DateTimeOffset.Now);
                await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host {HostName} {State} {DateTime}", HostAssemblyName, LifeTimeState.Failed, DateTimeOffset.Now);

                return 1;
            }
            finally
            {
                Log.Information("Host {HostName} {State} {DateTime}", HostAssemblyName, LifeTimeState.Stopping, DateTimeOffset.Now);
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(
                    (hostBuilderContext, serviceProvider, loggerConfiguration) =>
                        loggerConfiguration
                            .ReadFrom.Configuration(hostBuilderContext.Configuration)
                            .ReadFrom.Services(serviceProvider))
                .ConfigureWebHostDefaults(
                    webBuilder =>
                            webBuilder.UseStartup<Startup>());
    }
}