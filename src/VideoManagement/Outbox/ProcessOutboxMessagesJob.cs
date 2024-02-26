namespace VideoManagement.Outbox;

// project PowerTrainer.Common.Outbox that will contain all the outbox related stuff
[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IPublisher _publisher;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Processing outbox messages");
        using var connection = await _sqlConnectionFactory.CreateConnectionAsync(context.CancellationToken);
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction, context.CancellationToken);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var type = System.Type.GetType(outboxMessage.Type)
                           ?? throw new InvalidOperationException($"Cannot find type {outboxMessage.Type}");
                var message = (IDomainEvent)(JsonSerializer.Deserialize(outboxMessage.Data, type)
                                             ?? throw new InvalidOperationException($"Cannot deserialize message into type {type}"));
                await _publisher.Publish(message, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing outbox message {Id}", outboxMessage.Id);
                exception = ex;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception, context.CancellationToken);
        }

        transaction.Commit();
        _logger.LogInformation("Processing outbox messages finished");
    }

    private static async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        const string sql = """
                               UPDATE outbox_messages
                               SET processed_on_utc = @ProcessedOnUtc,
                                   error = @Error
                               WHERE id = @Id
                           """;

        _ = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    outboxMessage.Id,
                    ProcessedOnUtc = DateTime.UtcNow,
                    Error = exception?.ToString(),
                },
                transaction: transaction,
                cancellationToken: cancellationToken));
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        string sql = $"""
                          SELECT id, data, type
                          FROM outbox_messages
                          WHERE processed_on_utc IS NULL
                          ORDER BY occured_on_utc
                          LIMIT {_outboxOptions.BatchSize}
                          FOR UPDATE
                      """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            new CommandDefinition(
            sql,
            transaction: transaction,
            cancellationToken: cancellationToken));

        return outboxMessages.ToList();
    }
}
