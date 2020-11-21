namespace Template_NET_CORE_3_WORKER.Components.MassTransit.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using Template_NET_CORE_3_WORKER.Components.MassTransit.StateMachines;
    using Template_NET_CORE_3_WORKER.Contracts;

    public class RequestOfferActivity : Activity<OfferState, OfferRequested>
    {
        private readonly ILogger<RequestOfferActivity> _log;

        public RequestOfferActivity(ILogger<RequestOfferActivity> log)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("request-offer");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OfferState, OfferRequested> context, Behavior<OfferState, OfferRequested> next)
        {
            this._log.LogInformation("Executing Activity {@Data}", context.Data);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OfferState, OfferRequested, TException> context, Behavior<OfferState, OfferRequested> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
