namespace VideoManagement.Test.Unit.Validators;

public class StartVideoUploadCommandValidatorTests
{
    private readonly StartVideoUploadCommandValidator _sut = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new StartVideoUploadCommand(
            new StartVideoUpload("Valid Video Name", "Valid Owner Name"));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyVideoName_ShouldHaveErrors()
    {
        // Arrange
        var command = new StartVideoUploadCommand(
            new StartVideoUpload(string.Empty, "Valid Owner Name"));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void Validate_WithEmptyOwnerName_ShouldHaveErrors()
    {
        // Arrange
        var command = new StartVideoUploadCommand(
            new StartVideoUpload("Valid Video Name", string.Empty));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void Validate_WithExceedingLength_ShouldHaveErrors()
    {
        // Arrange
        string longName = new('a', 256);
        var command = new StartVideoUploadCommand(
            new StartVideoUpload(longName, longName));

        // Act
        var result = _sut.Validate(command);

        // Assert
        _ = result.Errors.Should().HaveCount(2);
    }
}
