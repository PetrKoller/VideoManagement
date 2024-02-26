﻿global using Common.Auth;
global using Common.Auth.TokenServices;
global using Common.Test.Integration.Setup;
global using FluentAssertions;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System.Net;
global using System.Text.Json;
global using TestingApp;
global using WireMock.Matchers;
global using WireMock.RequestBuilders;
global using WireMock.ResponseBuilders;
global using WireMock.Server;
global using Xunit;