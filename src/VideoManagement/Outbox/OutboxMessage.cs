namespace VideoManagement.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occuredOnUtc, string type, string data)
    {
        Id = id;
        OccuredOnUtc = occuredOnUtc;
        Type = type;
        Data = data;
    }

    public Guid Id { get; init; }

    public DateTime OccuredOnUtc { get; init; }

    public string Type { get; init; }

    public string Data { get; init; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }
}
