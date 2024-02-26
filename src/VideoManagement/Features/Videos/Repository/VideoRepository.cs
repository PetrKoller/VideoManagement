namespace VideoManagement.Features.Videos.Repository;

public sealed class VideoRepository : Repository<Video, AppDbContext>, IVideoRepository
{
    public VideoRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Video?> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default) =>
        DbContext.Videos.FirstOrDefaultAsync(
            video => video.EncodingJobId == jobId, cancellationToken);
}
