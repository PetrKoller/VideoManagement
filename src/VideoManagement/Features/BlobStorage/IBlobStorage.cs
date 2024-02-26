namespace VideoManagement.Features.BlobStorage;

public interface IBlobStorage
{
    string GenerateUploadUrl(string blobName, double durationInMinutes = 30);
}
