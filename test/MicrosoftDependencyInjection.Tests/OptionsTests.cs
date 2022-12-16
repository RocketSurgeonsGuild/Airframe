using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.DefaultOptions;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.FlatOptions;
using Xunit;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests
{
    public class OptionsTests
    {
        [Theory]
        [ClassData(typeof(DefaultOptionsTestData))]
        public void GivenDefaultOptions_WhenGetOptions_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<TransientFaultHandlingOptions>>()!
               .Value
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(DefaultOptionsTestData))]
        public void GivenDefaultOptions_WhenGetOptions_ThenStuffHasValue(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<TransientFaultHandlingOptions>>()!
               .Value
               .Enabled
               .Should()
               .BeTrue();

        [Theory]
        [ClassData(typeof(FlatOptionsTestData))]
        public void GivenFlatSettings_WhenGetOptions_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<FlatSettings>>()!
               .Value
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(FlatOptionsTestData))]
        public void GivenFlatSettings_WhenGetOptions_ThenStuffHasValue(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<FlatSettings>>()!
               .Value
               .Stuff
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(FlatOptionsTestData))]
        public void GivenFlatSettings_WhenGetOptions_ThenThingsHasValue(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<FlatSettings>>()!
               .Value
               .Things
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(AncestorOptionsTestData))]
        public void GivenLevelSettings_WhenGetOptions_ThenOptionsNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<LevelSettings>>()!
               .Value
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(AncestorOptionsTestData))]
        public void GivenLevelSettings_WhenGetOptions_ThenGenerationsNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<LevelSettings>>()!
               .Value
               .Generations
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(AncestorOptionsTestData))]
        public void GivenLevelSettings_WhenGetOptions_ThenParentNotNull(IServiceProvider serviceProvider) =>

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
        public void GivenLevelSettings_WhenGetOptions_ThenComplexNotNull(IServiceProvider serviceProvider) =>

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
        public void GivenLevelSettings_WhenGetOptions_ThenThingNotNull(IServiceProvider serviceProvider) =>

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

        [Theory]
        [ClassData(typeof(ConfigurationOptionsTestData))]
        public void GivenUsedConfigurationOptions_WhenGetOptions_ThenThingNotNull(IServiceProvider serviceProvider) =>

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

        [Theory]
        [ClassData(typeof(MultipleConfigurationOptionsTestData))]
        public void GivenMultipleConfigurationSources_WhenGetOptions_ThenThingNotNull(IServiceProvider serviceProvider)
        {
            // Given, When, Then
            serviceProvider
               .GetService<IOptions<FlatSettings>>()!
               .Value
               .Stuff
               .Should()
               .NotBeNull();

            serviceProvider
               .GetService<IOptions<TransientFaultHandlingOptions>>()!
               .Value
               .Enabled
               .Should()
               .BeTrue();

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

        [Theory]
        [ClassData(typeof(ConfigurationBuilderTestData))]
        public void GivenRegisteredConfigurationBuilder_WhenGetService_ThenNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<Action<IConfigurationBuilder>>()!
               .Should()
               .NotBeNull();

        [Theory]
        [ClassData(typeof(ConfigurationBuilderTestData))]
        public void GivenRegisteredConfigurationBuilder_WhenGetOptions_ThenNotNull(IServiceProvider serviceProvider) =>

            // Given, When, Then
            serviceProvider
               .GetService<IOptions<LevelSettings>>()!
               .Should()
               .NotBeNull();
    }
}