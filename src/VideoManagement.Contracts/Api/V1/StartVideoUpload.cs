namespace VideoManagement.Contracts.Api.V1;

public record StartVideoUpload(
    string VideoName,
    string OwnerName,
    bool IsDownloadable = false);
