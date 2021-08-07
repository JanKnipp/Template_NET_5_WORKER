namespace Template_NET_5_WORKER.Components.MassTransit.Consumers
{
    using global::MassTransit;
    using global::MassTransit.ConsumeConfigurators;
    using global::MassTransit.Definition;

    using GreenPipes;

    public class OfferConsumerDefinition : ConsumerDefinition<OfferConsumer>
    {
        public OfferConsumerDefinition()
        {
            this.ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OfferConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(configurator => configurator.Interval(5, 1000));
        }
    }
}