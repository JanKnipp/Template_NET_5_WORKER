namespace Template_NET_5_WORKER.CoreService.Models.Configuration
{
    internal class SampleQuartzServiceOptions
    {
        public string CronConfig { get; set; } = "0 0 0 1 1 ? 2099";
    }
}