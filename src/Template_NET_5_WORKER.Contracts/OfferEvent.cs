namespace Template_NET_5_WORKER.Contracts
{
    using System;

    public interface OfferEvent
    {
        string Text { get; }
        Guid OfferId { get; }
        DateTime TimeStamp { get; }
    }
}