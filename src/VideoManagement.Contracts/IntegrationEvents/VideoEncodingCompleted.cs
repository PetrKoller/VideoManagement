namespace VideoManagement.Contracts.IntegrationEvents;

public sealed record VideoEncodingCompleted(string OwnerName, Guid OwnerId, Guid VideoId);
