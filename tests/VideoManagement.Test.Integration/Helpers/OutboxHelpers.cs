namespace VideoManagement.Test.Integration.Helpers;

public static class OutboxHelpers
{
    public static async Task<IReadOnlyList<OutboxMessage>> GetAllOutboxMessages(
        this ISqlConnectionFactory factory)
    {
        using var connection = await factory.CreateConnectionAsync(default);
        return (await connection.QueryAsync<OutboxMessage>("SELECT id, occured_on_utc AS occuredOnUtc,type,data FROM outbox_messages")).ToList();
    }
}
