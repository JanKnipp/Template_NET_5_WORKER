namespace Template_NET_5_WORKER.CoreService.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    using OpenTelemetry.Trace;

    public static class CustomOpenTelemetry
    {
        public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection collection)
        {
            return collection.AddOpenTelemetryTracing(
                (provider, builder) =>
                    {
                        builder.AddAspNetCoreInstrumentation();
                        builder.AddConsoleExporter();
                    });
        }
    }
}