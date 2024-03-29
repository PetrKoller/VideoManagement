﻿global using Amazon.MediaConvert;
global using Amazon.MediaConvert.Model;
global using Common.ResultTypes;
global using FluentAssertions;
global using Microsoft.Extensions.Logging;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using VideoManagement.Contracts.Api.V1;
global using VideoManagement.Features.Media;
global using VideoManagement.Features.Videos.DomainEvents;
global using VideoManagement.Features.Videos.Encode.EncodingCompleted;
global using VideoManagement.Features.Videos.Encode.MediaConvertEvents;
global using VideoManagement.Features.Videos.Entity;
global using VideoManagement.Features.Videos.Upload;
global using VideoManagement.Options;
global using Xunit;
