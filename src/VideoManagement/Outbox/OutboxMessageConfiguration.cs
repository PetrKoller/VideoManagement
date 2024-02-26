namespace VideoManagement.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        _ = builder.ToTable("outbox_messages");
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.Data).HasColumnType("json");
    }
}
