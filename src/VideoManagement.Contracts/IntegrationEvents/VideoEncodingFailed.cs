namespace VideoManagement.Contracts.IntegrationEvents;

public sealed record VideoEncodingFailed(string OwnerName, Guid OwnerId, Guid VideoId);
