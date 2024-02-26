namespace VideoManagement.Features.Media;

public class CreateJobRequestBuilder
{
    private readonly CreateJobRequest _request;

    public CreateJobRequestBuilder()
    {
        _request = new CreateJobRequest();
        InitDefaults();
    }

    public CreateJobRequestBuilder WithRole(string role)
    {
        _request.Role = role;
        return this;
    }

    public CreateJobRequest Build()
        => _request;

    public CreateJobRequestBuilder WithInput(string input)
    {
        _request.Settings.Inputs.Add(new Input
        {
            FileInput = input,
            AudioSelectors = new()
            {
                {
                    "Audio Selector 1", new AudioSelector
                    {
                        DefaultSelection = AudioDefaultSelection.DEFAULT,
                    }
                },
            },
            VideoSelector = new VideoSelector
            {
                Rotate = InputRotate.AUTO,
            },
            TimecodeSource = InputTimecodeSource.ZEROBASED,
        });

        return this;
    }

    public CreateJobRequestBuilder WithHlsOutputGroup(string destination)
    {
        var hlsOutputGroup = new OutputGroup
        {
            Name = "Apple HLS",
            Outputs =
            [
                new()
                {
                    ContainerSettings = new ContainerSettings { Container = ContainerType.M3U8, },
                    VideoDescription = GetVideoDescription(),
                    AudioDescriptions = GetAudioDescription(),
                    NameModifier = "-stream",
                }

            ],
            OutputGroupSettings = new OutputGroupSettings
            {
                Type = OutputGroupType.HLS_GROUP_SETTINGS,
                HlsGroupSettings = new HlsGroupSettings
                {
                    SegmentLength = 10,
                    MinSegmentLength = 0,
                    Destination = destination,
                },
            },
        };

        _request.Settings.OutputGroups.Add(hlsOutputGroup);

        return this;
    }

    public CreateJobRequestBuilder WithFileOutput(string destination)
    {
        var fileOutputGroup = new OutputGroup
        {
            Name = "File Group",
            Outputs =
            [
                new()
                {
                    ContainerSettings = new ContainerSettings { Container = ContainerType.MP4, },
                    VideoDescription = GetVideoDescription(),
                    AudioDescriptions = GetAudioDescription(),
                    Extension = "mp4",
                    NameModifier = "-download",
                }

            ],
            OutputGroupSettings = new OutputGroupSettings
            {
                Type = OutputGroupType.FILE_GROUP_SETTINGS,
                FileGroupSettings =
                    new FileGroupSettings { Destination = destination, },
            },
        };

        _request.Settings.OutputGroups.Add(fileOutputGroup);

        return this;
    }

    public CreateJobRequestBuilder WithTag(string key, string value)
    {
        _request.Tags.Add(key, value);
        return this;
    }

    private static VideoDescription GetVideoDescription()
        => new()
        {
            CodecSettings = new VideoCodecSettings
            {
                Codec = VideoCodec.H_264,
                H264Settings = new H264Settings
                {
                    FramerateDenominator = 1001,
                    MaxBitrate = 1000000,
                    FramerateControl = H264FramerateControl.SPECIFIED,
                    RateControlMode = H264RateControlMode.QVBR,
                    FramerateNumerator = 24000,
                    SceneChangeDetect = H264SceneChangeDetect.TRANSITION_DETECTION,
                    QualityTuningLevel = H264QualityTuningLevel.SINGLE_PASS,
                },
            },
        };

    private static List<AudioDescription> GetAudioDescription()
        =>
        [
            new()
            {
                CodecSettings = new AudioCodecSettings
                {
                    Codec = AudioCodec.AAC,
                    AacSettings =
                        new AacSettings
                        {
                            Bitrate = 96000,
                            CodingMode = AacCodingMode.CODING_MODE_2_0,
                            SampleRate = 48000,
                        },
                },
            }

        ];

    private void InitDefaults()
    {
        _request.AccelerationSettings = new AccelerationSettings { Mode = "DISABLED", };
        _request.Priority = 0;
        _request.Settings = new JobSettings();
    }
}
