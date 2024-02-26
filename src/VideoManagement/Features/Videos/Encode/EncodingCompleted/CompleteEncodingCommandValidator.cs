namespace VideoManagement.Features.Videos.Encode.EncodingCompleted;

public class CompleteEncodingCommandValidator : AbstractValidator<CompleteEncodingCommand>
{
    public CompleteEncodingCommandValidator()
    {
        _ = RuleFor(x => x.Event.Detail)
            .NotEmpty();

        _ = RuleFor(x => x.Event.Detail.JobId)
            .NotEmpty();

        _ = RuleFor(x => x.Event.Detail.OutputGroupDetails)
            .NotEmpty()
            .Must(x =>
                x.Any(y => y.Type == MediaConvertCompletedEvent.HlsGroupName) &&
                (x.Count <= 1 || x.Any(y => y.Type == MediaConvertCompletedEvent.FileGroupName)))
            .WithMessage("OutputGroupDetails must contain HLS_GROUP and optionally FILE_GROUP");

        _ = RuleForEach(x => x.Event.Detail.OutputGroupDetails)
            .SetValidator(new OutputGroupDetailValidator());
    }
}
