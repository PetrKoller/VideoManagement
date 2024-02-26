namespace VideoManagement.Outbox;

public record OutboxMessageResponse(Guid Id, string Data, string Type);
