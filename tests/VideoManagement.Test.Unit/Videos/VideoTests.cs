namespace VideoManagement.Test.Unit.Videos;

public class VideoTests
{
    [Fact]
    public void Video_RaisesEncodingCompletedEvent_WhenMarkedAsCompleted()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "Test",
            "Test",
            Guid.NewGuid(),
            false,
            DateTime.UtcNow);

        // Act
        video.EncodingCompleted(DateTime.UtcNow, "testStreamingPath");
        var encodingCompleted = video.GetDomainEvents().OfType<EncodingCompletedDomainEvent>().SingleOrDefault();

        // Assert
        _ = encodingCompleted.Should().NotBeNull();
        _ = encodingCompleted!.VideoId.Should().Be(video.ExternalId);
        _ = video.Status.Should().Be(VideoStatus.Completed);
    }

    [Fact]
    public void Video_RaiseEncodingFailedEvent_WhenMarkedAsFailed()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "Test",
            "Test",
            Guid.NewGuid(),
            false,
            DateTime.UtcNow);

        const string expectedMsg = "Test error";

        // Act
        video.EncodingFailed(expectedMsg);
        var encodingCompleted = video.GetDomainEvents().OfType<EncodingFailedDomainEvent>().SingleOrDefault();

        // Assert
        _ = encodingCompleted.Should().NotBeNull();
        _ = encodingCompleted!.VideoId.Should().Be(video.ExternalId);
        _ = video.Status.Should().Be(VideoStatus.Failed);
    }
}
