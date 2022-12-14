using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections;

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
           .Stuff
           .Should()
           .NotBeNull();
}

internal class DefaultOptionsTestData : IEnumerable<object[]>
{
    public DefaultOptionsTestData()
    {
        var serviceCollection = new ServiceCollection();
        _buildServiceProvider = serviceCollection
           .ConfigureAppSettings(builder => builder.AddJsonFile("defaultoptions.json", optional: false))
           .ConfigureSectionAsOptions<TransientFaultHandlingOptions>()
           .BuildServiceProvider();
    }

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly IServiceProvider _buildServiceProvider;
}

internal class FlatOptionsTestData : IEnumerable<object[]>
{
    public FlatOptionsTestData()
    {
        var serviceCollection = new ServiceCollection();
        _buildServiceProvider = serviceCollection
           .ConfigureAppSettings(builder => builder.AddJsonFile("flatsettings.json", optional: false))
           .ConfigureOptions<FlatSettings>()
           .BuildServiceProvider();
    }

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly IServiceProvider _buildServiceProvider;
}