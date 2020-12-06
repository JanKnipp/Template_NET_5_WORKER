namespace Template_NET_5_WORKER.CoreService
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Template_NET_5_WORKER.CoreService.Configuration;
    using Template_NET_5_WORKER.CoreService.HostedServices;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<LifeTimeEventService>();

            services.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromSeconds(15); });
            services.Configure<KestrelServerOptions>(this._configuration.GetSection("Kestrel"));

            services.AddCustomOpenTelemetry();
            services.AddCustomHealthChecks();
            services.AddCustomMassTransit();
            services.AddCustomQuartz(this._configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        private void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            app.UseCustomHealthChecks();
        }

        /// <summary>
        /// Configuration for Development environment
        /// </summary>
        /// <param name="app"></param>
        /// <param name="applicationLifetime"></param>
        public void ConfigureDevelopment(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            this.Configure(app, applicationLifetime);
        }

        /// <summary>
        /// Configuration for Production environment
        /// </summary>
        /// <param name="app"></param>
        /// <param name="applicationLifetime"></param>
        public void ConfigureProduction(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            this.Configure(app, applicationLifetime);
        }
    }
}