namespace VideoManagement.Features.Videos.Repository;

public interface IVideoRepository
{
    Task<Video> AddAsync(Video video, CancellationToken cancellationToken = default);

    Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Video?> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default);
}
