namespace VideoManagement.Features.Videos.Encode.EncodingCompleted;

public sealed class EncodingCompletedConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly SqsOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EncodingFailedConsumer> _logger;

    public EncodingCompletedConsumer(
        IAmazonSQS sqsClient,
        ILogger<EncodingFailedConsumer> logger,
        IOptions<SqsOptions> options,
        IServiceProvider serviceProvider)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_options.EncodingCompletedQueue, stoppingToken);

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
                    _logger.LogError(e, "Unexpected error while handling encoding completed event. Message: {Message}", message);
                }
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, string queueUrl, CancellationToken cancellationToken)
    {
        var completedEvent = JsonSerializer.Deserialize<MediaConvertCompletedEvent>(message.Body);

        if (completedEvent is not null && completedEvent.IsVideoEncodingCompletedEvent())
        {
            var result = await HandleEncodingCompletedEventAsync(completedEvent, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error while handling encoding completed event. Error: {Error}", result.Failure.Message);
                return;
            }

            _ = await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Received message is not a valid encoding completed event. Message: {Message}", message);
        }
    }

    private async Task<OperationResult<Success>> HandleEncodingCompletedEventAsync(MediaConvertCompletedEvent completedEvent, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(new CompleteEncodingCommand(completedEvent), cancellationToken);
    }
}
