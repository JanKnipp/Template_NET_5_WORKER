namespace Template_NET_CORE_3_WORKER.CoreService.MassTransitConsumers
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.Extensions.Logging;

    using Template_NET_CORE_3_WORKER.CoreService.Models;

    public class SampleConsumer : IConsumer<SampleRequest>
    {
        private readonly ILogger<SampleConsumer> _log;
        private readonly string _consumerName;

        public SampleConsumer(ILogger<SampleConsumer> log)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._consumerName = this.GetType().Name;
        }

        public async Task Consume(ConsumeContext<SampleRequest> context)
        {
            this._log.LogDebug("{ConsumerName} executing", this._consumerName);
        }
    }
}