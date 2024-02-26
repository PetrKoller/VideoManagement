namespace Common.ResultTypes;

/// <summary>
/// Represents a result that can be either a <typeparamref name="T" /> or a <see cref="Failure" />.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
[GenerateOneOf]
public sealed partial class OperationResult<T> : OneOfBase<T, Failure>
{
    public bool IsFailure => IsT1;

    public Failure Failure => AsT1;

    public T Data => AsT0;

    public bool TryPickFailure(out Failure value, out T remainder)
        => TryPickT1(out value, out remainder);

    public bool TryPickData(out T value, out Failure remainder)
        => TryPickT0(out value, out remainder);
}
