namespace VideoManagement.Features.Videos.Encode.EncodingCompleted;

public class OutputGroupDetailValidator : AbstractValidator<OutputGroupDetail>
{
    public OutputGroupDetailValidator()
        => RuleFor(x => x.OutputDetails)
            .NotEmpty()
            .WithMessage("OutputDetails must contain at least one OutputFilePaths")
            .Must(x => x.All(y => y.OutputFilePaths.Any()))
            .WithMessage("OutputFilePaths must not be empty");
}
