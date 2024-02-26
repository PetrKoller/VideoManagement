namespace VideoManagement.Test.Unit.Media;

public class MediaServiceTests
{
    private readonly MediaService _sut;
    private readonly ILogger<MediaService> _logger = Substitute.For<ILogger<MediaService>>();
    private readonly IAmazonMediaConvert _mediaConvert = Substitute.For<IAmazonMediaConvert>();

    public MediaServiceTests()
    {
        var blobStorageOptions = Microsoft.Extensions.Options.Options.Create(
            new BlobStorageOptions { BucketName = "Test" });
        var mediaServiceOptions = Microsoft.Extensions.Options.Options.Create(
            new MediaServiceOptions
            {
                MediaConvertRole = "Test",
                MediaConvertEndpoint = "/testEndpint",
            });
        _sut = new MediaService(_mediaConvert, mediaServiceOptions, blobStorageOptions, _logger);
    }

    [Fact]
    public async Task CreateEncodingJobAsync_ReturnsFailure_WhenJobsFails()
    {
        // Arrange
        _ = _mediaConvert.CreateJobAsync(Arg.Any<CreateJobRequest>())
            .ThrowsAsync(new AmazonMediaConvertException("exception"));
        var expectedResult = new OperationResult<string>(MediaServiceErrors.JobCreationFailed());

        // Act
        var result = await _sut.CreateEncodingJobAsync(Guid.NewGuid(), "test", "test", false);

        // Assert
        _ = result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task CreateEncodingJobAsync_ReturnsString_WhenJobStarted()
    {
        // Arrange
        const string testId = "testId";
        var outputGroups = new List<OutputGroup>();
        _ = _mediaConvert.CreateJobAsync(Arg.Do<CreateJobRequest>(x => outputGroups = x.Settings.OutputGroups))
            .Returns(new CreateJobResponse { Job = new Job { Id = testId } });
        var expectedResult = new OperationResult<string>(testId);

        // Act
        var result = await _sut.CreateEncodingJobAsync(Guid.NewGuid(), "test", "test", false);

        _ = result.Should().Be(expectedResult);
        _ = outputGroups.Should().HaveCount(1);
        _ = outputGroups[0].OutputGroupSettings.Type.Should().Be(OutputGroupType.HLS_GROUP_SETTINGS);
    }

    [Fact]
    public async Task CreateEncodingJobAsync_AddsFileGroupToRequest_WhenDownloadable()
    {
        // Arrange
        const string testId = "testId";
        var outputGroups = new List<OutputGroup>();
        _ = _mediaConvert.CreateJobAsync(Arg.Do<CreateJobRequest>(x => outputGroups = x.Settings.OutputGroups))
            .Returns(new CreateJobResponse { Job = new Job { Id = testId } });
        var expectedResult = new OperationResult<string>(testId);

        // Act
        var result = await _sut.CreateEncodingJobAsync(Guid.NewGuid(), "test", "test", true);

        _ = result.Should().Be(expectedResult);
        _ = outputGroups.Should().HaveCount(2);
        _ = outputGroups[0].OutputGroupSettings.Type.Should().Be(OutputGroupType.HLS_GROUP_SETTINGS);
        _ = outputGroups[1].OutputGroupSettings.Type.Should().Be(OutputGroupType.FILE_GROUP_SETTINGS);
    }
}
