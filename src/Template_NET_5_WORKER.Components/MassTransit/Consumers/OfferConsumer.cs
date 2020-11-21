namespace Template_NET_5_WORKER.Components.MassTransit.Consumers
{
    using System;
    using System.Threading.Tasks;

    using global::MassTransit;

    using Microsoft.Extensions.Logging;

    using Template_NET_5_WORKER.Contracts;

    public class OfferConsumer : IConsumer<IOfferRequest>
    {
        private readonly ILogger<OfferConsumer> _log;
        private readonly string _consumerName;

        public OfferConsumer(ILogger<OfferConsumer> log)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._consumerName = this.GetType().Name;
        }

        public async Task Consume(ConsumeContext<IOfferRequest> context)
        {
            this._log.LogDebug("{ConsumerName} executing with message {@Message}", this._consumerName, context.Message);

            await context
                .Publish<OfferRequested>(
                    new { context.Message.OfferId, context.Message.Text, context.Message.TimeStamp })
                .ConfigureAwait(false);
        }
    }
}