using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template_NET_5_WORKER.CoreService.Helper
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public static class HealthCheckResponseWriter
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(entry => new JProperty(entry.Key, new JObject(
                    new JProperty("status", entry.Value.Status.ToString()),
                    new JProperty("description", entry.Value.Description),
                    new JProperty("data", JObject.FromObject(entry.Value.Data))))))));

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(json.ToString(Formatting.Indented), context.RequestAborted);
        }
    }
}