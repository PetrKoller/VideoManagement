namespace Common.ResultTypes;

/// <summary>
/// Simple record type representing a failure.
/// </summary>
/// <param name="Code">Code describing the failure.</param>
/// <param name="Message">Message describing the failure.</param>
public record Failure(string Code, string Message);
