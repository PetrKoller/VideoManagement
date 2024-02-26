namespace VideoManagement.Outbox;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}
