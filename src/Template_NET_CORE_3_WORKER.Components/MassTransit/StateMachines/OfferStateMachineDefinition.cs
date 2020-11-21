namespace Template_NET_CORE_3_WORKER.Components.MassTransit.StateMachines
{
    using System;

    using global::MassTransit;
    using global::MassTransit.Definition;

    using GreenPipes;

    public class OfferStateMachineDefinition: SagaDefinition<OfferState>
    {
        public OfferStateMachineDefinition()
        {
            this.ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OfferState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(99, TimeSpan.FromSeconds(5)));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}