namespace Common.Validations;

/// <summary>
/// Validation related extensions.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Registers the validation behavior as the scoped service for the specified request and result types.
    /// </summary>
    /// <param name="services">Service collection where the validation behavior will be registered.</param>
    /// <typeparam name="TRequest">Mediator Request to be validated.</typeparam>
    /// <typeparam name="TResult">Expected <see cref="OperationResult{T}"/> of the TRequest. </typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddValidationBehavior<TRequest, TResult>(this IServiceCollection services)
        where TRequest : IMessage
        => services
            .AddScoped<
                IPipelineBehavior<TRequest, OperationResult<TResult>>,
                ValidationBehavior<TRequest, TResult>>();
}
