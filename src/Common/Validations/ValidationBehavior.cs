namespace Common.Validations;

/// <summary>
/// Mediator pipeline behavior for validation.
/// </summary>
/// <typeparam name="TRequest">Mediator Request to be validated.</typeparam>
/// <typeparam name="TResult">Expected <see cref="OperationResult{T}"/> of the TRequest, so the proper
/// <see cref="Failure"/> can be returned instead of throwing exceptions.</typeparam>
public class ValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, OperationResult<TResult>>
    where TRequest : IMessage
{
    private const string DefaultErrorMessage = "Validation error occurred.";
    private const string ErrorCode = "Validation.Error";
    private readonly IValidator<TRequest> _validator;
    private readonly ILogger<ValidationBehavior<TRequest, TResult>> _logger;

    public ValidationBehavior(
        IValidator<TRequest> validator,
        ILogger<ValidationBehavior<TRequest, TResult>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<OperationResult<TResult>> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, OperationResult<TResult>> next)
    {
        _logger.LogDebug("Triggered validation behavior for message: {Message}", typeof(TRequest).FullName);
        var validationResult = await _validator.ValidateAsync(message, cancellationToken);

        return validationResult.IsValid
            ? await next(message, cancellationToken)
            : new Failure(ErrorCode, validationResult.ToString() ?? DefaultErrorMessage);
    }
}
