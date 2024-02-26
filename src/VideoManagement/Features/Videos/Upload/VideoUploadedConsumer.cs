namespace VideoManagement.Features.Videos.Upload;

public sealed class VideoUploadedConsumer : BackgroundService
{
    private readonly SqsOptions _options;
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<VideoUploadedConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public VideoUploadedConsumer(
        IOptions<SqsOptions> sqsOptions,
        IAmazonSQS sqsClient,
        ILogger<VideoUploadedConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _options = sqsOptions.Value;
        _sqsClient = sqsClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_options.VideoUploadedQueue, stoppingToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = ["All"],
            MessageAttributeNames = ["All"],
            WaitTimeSeconds = _options.WaitTimeSeconds,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

            foreach (var message in response.Messages)
            {
                _logger.LogDebug("Message received: {Message}", message);
                try
                {
                    await ProcessMessageAsync(message, queueUrlResponse.QueueUrl, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error while handling video uploaded event. Message: {Message}", message);
                }
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, string queueUrl, CancellationToken cancellationToken)
    {
        var s3Event = JsonSerializer.Deserialize<S3Event>(message.Body);

        if (s3Event is not null && s3Event.IsVideoUploadedEvent())
        {
            var result = await HandleVideoUploadedEventAsync(s3Event, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error while handling video uploaded event. Error: {Error}", result.Failure.Message);
                return;
            }

            _ = await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Received message is not a valid video uploaded event. Message: {Message}", message);
        }
    }

    private async Task<OperationResult<Success>> HandleVideoUploadedEventAsync(S3Event s3Event, CancellationToken cancellationToken)
    {
        string videoKey = s3Event.GetVideoKey();
        var videoId = Guid.Parse(videoKey.Split("/")[2]);

        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(new StartVideoEncodingCommand(videoId), cancellationToken);
    }
}
