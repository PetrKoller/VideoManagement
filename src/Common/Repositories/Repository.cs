namespace Common.Repositories;

/// <summary>
/// Provides a base repository for entities using Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TContext">The type of the DbContext.</typeparam>
public abstract class Repository<TEntity, TContext>
    where TEntity : Entity
    where TContext : DbContext
{
    protected Repository(TContext dbContext)
        => DbContext = dbContext;

    protected TContext DbContext { get; }

    /// <summary>
    /// Gets the entity by its external identifier.
    /// </summary>
    /// <param name="id">The external identifier of the entity.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the entity found, or null.
    /// </returns>
    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.ExternalId == id, cancellationToken);

    /// <summary>
    /// Asynchronously adds the specified entity to the DbContext and returns the tracked entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains the entity after it has been added to the DbContext.
    /// </returns>
    public async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
        => (await DbContext.AddAsync(entity, cancellationToken)).Entity;
}
