using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.DefaultOptions;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.FlatOptions;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests;

public class OptionsTests
{
    [Theory]
    [ClassData(typeof(DefaultOptionsTestData))]
    public void GivenDefaultOptions_WhenGetService_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<TransientFaultHandlingOptions>>()!
           .Value
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(DefaultOptionsTestData))]
    public void GivenDefaultOptions_WhenGetService_ThenStuffHasValue(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<TransientFaultHandlingOptions>>()!
           .Value
           .Enabled
           .Should()
           .BeTrue();

    [Theory]
    [ClassData(typeof(FlatOptionsTestData))]
    public void GivenFlatSettings_WhenGetService_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<FlatSettings>>()!
           .Value
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(FlatOptionsTestData))]
    public void GivenFlatSettings_WhenGetService_ThenStuffHasValue(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<FlatSettings>>()!
           .Value
           .Stuff
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(FlatOptionsTestData))]
    public void GivenFlatSettings_WhenGetService_ThenThingsHasValue(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<FlatSettings>>()!
           .Value
           .Things
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(AncestorOptionsTestData))]
    public void GivenLevelSettings_WhenGetService_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<LevelSettings>>()!
           .Value
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(AncestorOptionsTestData))]
    public void GivenLevelSettings_WhenGetService_ThenGenerationsNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<LevelSettings>>()!
           .Value
           .Generations
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(AncestorOptionsTestData))]
    public void GivenLevelSettings_WhenGetService_ThenParentNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<LevelSettings>>()!
           .Value
           .Generations
           .Parent
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(AncestorOptionsTestData))]
    public void GivenLevelSettings_WhenGetService_ThenComplexNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<LevelSettings>>()!
           .Value
           .Generations
           .Parent
           .Complex
           .Should()
           .NotBeNull();

    [Theory]
    [ClassData(typeof(AncestorOptionsTestData))]
    public void GivenLevelSettings_WhenGetService_ThenThingNotNull(IServiceProvider serviceProvider) =>

        // Given, When, Then
        serviceProvider
           .GetService<IOptions<LevelSettings>>()!
           .Value
           .Generations
           .Parent
           .Complex
           .Thing
           .Should()
           .NotBeNull();
}