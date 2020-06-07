namespace Template_NET_CORE_3_WORKER.CoreService.MassTransitConsumers
{
    using GreenPipes;

    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;

    public class SampleConsumerDefinition : ConsumerDefinition<SampleConsumer>
    {

        public SampleConsumerDefinition()
        {
            this.ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SampleConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(configurator => configurator.Interval(5, 1000));
        }
    }
}