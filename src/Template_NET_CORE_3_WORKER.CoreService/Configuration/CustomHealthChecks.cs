namespace Template_NET_CORE_3_WORKER.CoreService.Configuration
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Template_NET_CORE_3_WORKER.CoreService.Helper;

    public static class CustomHealthChecks
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection collection)
        {
            collection.AddHealthChecks();

            collection.Configure<HealthCheckPublisherOptions>(options => { options.Delay = TimeSpan.FromSeconds(2); });

            return collection;
        }

        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                    {
                        Predicate = check => check.Tags.Contains("ready"),
                        ResponseWriter = HealthCheckResponseWriter.WriteResponse
                    });

            app.UseHealthChecks("/health/live", new HealthCheckOptions { ResponseWriter = HealthCheckResponseWriter.WriteResponse });

            return app;
        }
    }
}