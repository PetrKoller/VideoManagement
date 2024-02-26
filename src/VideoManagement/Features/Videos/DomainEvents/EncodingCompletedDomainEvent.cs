namespace VideoManagement.Features.Videos.DomainEvents;

public sealed record EncodingCompletedDomainEvent(string OwnerName, Guid OwnerId, Guid VideoId) : IDomainEvent;
