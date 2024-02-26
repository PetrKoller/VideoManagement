namespace VideoManagement.Features.Videos.DomainEvents;

public sealed class EncodingCompletedDomainEventHandler : INotificationHandler<EncodingCompletedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EncodingCompletedDomainEventHandler> _logger;

    public EncodingCompletedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<EncodingCompletedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async ValueTask Handle(EncodingCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Encoding completed for video {VideoId}, publishing an integration event", notification.VideoId);
        await _publishEndpoint.Publish(
            new VideoEncodingCompleted(
                notification.OwnerName,
                notification.OwnerId,
                notification.VideoId),
            cancellationToken);
    }
}
