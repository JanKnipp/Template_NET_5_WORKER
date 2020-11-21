namespace Template_NET_5_WORKER.Contracts
{
    using System;

    public interface IOfferRequest
    {
        string Text { get; }
        Guid OfferId { get; }
        DateTime TimeStamp { get; }

        int Number { get; }
    }
}