namespace Template_NET_5_WORKER.Components.MassTransit.StateMachines
{
    using System;

    using Automatonymous;

    using global::MassTransit.Saga;

    public class OfferState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public int Version { get; set; }

        public string CurrentState { get; set; }

        public DateTime DateTimeOccured { get; set; }
    }
}
