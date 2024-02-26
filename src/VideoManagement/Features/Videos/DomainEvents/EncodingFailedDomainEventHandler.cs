namespace VideoManagement.Features.Videos.DomainEvents;

public class EncodingFailedDomainEventHandler : INotificationHandler<EncodingFailedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EncodingFailedDomainEventHandler> _logger;

    public EncodingFailedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<EncodingFailedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async ValueTask Handle(
        EncodingFailedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Encoding failed for video {VideoId}, publishing an integration event", notification.VideoId);

        await _publishEndpoint.Publish(
            new VideoEncodingFailed(notification.OwnerName, notification.OwnerId, notification.VideoId),
            cancellationToken);
    }
}
