namespace Common.Abstractions;

/// <summary>
/// Entity base class.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(Guid externalId)
        => ExternalId = externalId;

    protected Entity()
    {
    }

    public int Id { get; init; }

    public Guid ExternalId { get; init; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _domainEvents.ToList();

    public void ClearDomainEvents()
        => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}
