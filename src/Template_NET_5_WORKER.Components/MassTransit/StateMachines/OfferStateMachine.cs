namespace Template_NET_5_WORKER.Components.MassTransit.StateMachines
{
    using System;

    using Automatonymous;

    using Template_NET_5_WORKER.Components.MassTransit.Activities;
    using Template_NET_5_WORKER.Contracts;

    public class OfferStateMachine:MassTransitStateMachine<OfferState>
    {
        public OfferStateMachine()
        {
            this.Event(() => this.OfferUpdated, configurator => configurator.CorrelateById(context => context.Message.OfferId));
            this.Event(() => this.OfferApproved, configurator => configurator.CorrelateById(context => context.Message.OfferId));
            this.Event(() => this.OfferCompleted, configurator => configurator.CorrelateById(context => context.Message.OfferId));
            this.Event(() => this.OfferRejected, configurator => configurator.CorrelateById(context => context.Message.OfferId));
            this.Event(() => this.OfferRequested, configurator => configurator.CorrelateById(context => context.Message.OfferId));

            this.InstanceState(state => state.CurrentState);

            this.Initially(
                this.When(this.OfferRequested)
                    .Then(context => context.Instance.DateTimeOccured = DateTime.UtcNow)
                    .Activity(selector => selector.OfType<RequestOfferActivity>())
                    .TransitionTo(this.Requested));

            this.During(
                this.Requested,
                this.Ignore(this.OfferRequested),
                this.When(this.OfferRejected).TransitionTo(this.Rejected),
                this.When(this.OfferApproved).TransitionTo(this.Approved));

            this.During(
                this.Approved,
                this.When(this.OfferCompleted).TransitionTo(this.Completed),
                this.When(this.OfferRejected).TransitionTo(this.Rejected));

            this.During(this.Rejected, this.When(this.OfferApproved).TransitionTo(this.Approved));
        }

        public State Requested { get; private set; }
        public State Reviewed { get; private set; }
        public State Rejected { get; private set; }
        public State Approved { get; private set; }
        public State Completed { get; private set; }


        public Event<OfferRequested> OfferRequested { get; private set; }
        public Event<OfferUpdated> OfferUpdated { get; private set; }
        public Event<OfferApproved> OfferApproved { get; private set; }
        public Event<OfferRejected> OfferRejected { get; private set; }
        public Event<OfferCompleted> OfferCompleted { get; private set; }
        
    }
}