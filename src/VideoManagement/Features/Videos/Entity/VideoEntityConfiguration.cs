namespace VideoManagement.Features.Videos.Entity;

public class VideoEntityConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        _ = builder.ToTable("videos");

        _ = builder.HasKey(c => c.Id);

        _ = builder
            .HasIndex(x => x.ExternalId)
            .IsUnique();

        _ = builder.HasIndex(x => x.EncodingJobId);

        _ = builder.Property(x => x.Name).HasMaxLength(255);
        _ = builder.Property(x => x.OwnerName).HasMaxLength(255);
    }
}
