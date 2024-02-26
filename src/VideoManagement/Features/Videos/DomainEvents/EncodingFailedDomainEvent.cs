namespace VideoManagement.Features.Videos.DomainEvents;

public sealed record EncodingFailedDomainEvent(string OwnerName, Guid OwnerId, Guid VideoId) : IDomainEvent;
