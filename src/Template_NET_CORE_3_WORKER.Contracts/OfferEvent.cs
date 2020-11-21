namespace Template_NET_CORE_3_WORKER.Contracts
{
    using System;

    public interface OfferEvent
    {
        string Text { get; }
        Guid OfferId { get; }
        DateTime TimeStamp { get; }
    }
}