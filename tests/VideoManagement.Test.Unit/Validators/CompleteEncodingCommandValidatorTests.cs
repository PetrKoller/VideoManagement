namespace VideoManagement.Test.Unit.Validators;

public class CompleteEncodingCommandValidatorTests
{
    private readonly CompleteEncodingCommandValidator _sut = new();

    [Fact]
    public void Validate_ReturnsTrue_WhenValidData()
    {
        // Arrange
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "jobId1",
                    "COMPLETE",
                    [
                        new(
                            [new(["test1"])],
                            MediaConvertCompletedEvent.HlsGroupName)

                    ])));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenValidData2()
    {
        // Arrange
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "jobId1",
                    "COMPLETE",
                    [
                        new(
                            [new(["test1"])],
                            MediaConvertCompletedEvent.HlsGroupName),

                        new(
                            [new(["test1"])],
                            MediaConvertCompletedEvent.FileGroupName)

                    ])));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_WhenDetailFieldIsEmpty()
    {
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "jobId1",
                    "COMPLETE",
                    [])));
        var result = _sut.Validate(command);
        _ = result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Fails_WhenJobIdIsEmpty()
    {
        // Arrange
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    string.Empty,
                    "COMPLETE",
                    [
                        new(
                            [new(["test1"])],
                            MediaConvertCompletedEvent.HlsGroupName)

                    ])));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Fails_WhenOutputGroupDetailsIsMissingHlsGroup()
    {
        // Arrange
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "jobId1",
                    "COMPLETE",
                    [
                        new(
                            [],
                            "SomeOtherType")

                    ])));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Fails_WhenOutputGroupDetailsWithMoreItemsDoesNotContainFileGroup()
    {
        // Arrange
        var command = new CompleteEncodingCommand(
            new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "jobId1",
                    "COMPLETE",
                    [
                        new(
                            [new(["test1"])],
                            MediaConvertCompletedEvent.HlsGroupName),

                        new(
                            [new(["test1"])],
                            "SomeOtherType")

                    ])));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.IsValid.Should().BeFalse();
    }
}
