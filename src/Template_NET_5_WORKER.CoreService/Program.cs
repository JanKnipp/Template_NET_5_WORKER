namespace Template_NET_5_WORKER.CoreService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using Serilog;

    using Template_NET_5_WORKER.CoreService.Models;

    public class Program
    {
        private static readonly string HostAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private static IConfigurationRoot _configuration;

        private static IConfigurationRoot GetConfiguration(string[] args)
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (args != null)
            {
                configBuilder.AddCommandLine(args);
            }

            return configBuilder.Build();
        }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                _configuration = GetConfiguration(args);

                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();

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
                .ConfigureHostConfiguration(
                    c =>
                        {
                            c.AddConfiguration(_configuration);
                        })
                .ConfigureWebHostDefaults(
                    webBuilder =>
                        {
                            webBuilder.UseStartup<Startup>();
                        })
                .UseSerilog();
    }
}