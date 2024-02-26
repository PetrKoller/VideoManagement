namespace VideoManagement.Features.Videos.Upload;

public sealed class StartVideoUploadCommandValidator : AbstractValidator<StartVideoUploadCommand>
{
    public StartVideoUploadCommandValidator()
    {
        _ = RuleFor(x => x.VideoUpload.VideoName).NotEmpty().MaximumLength(255);
        _ = RuleFor(x => x.VideoUpload.OwnerName).NotEmpty().MaximumLength(255);
    }
}
