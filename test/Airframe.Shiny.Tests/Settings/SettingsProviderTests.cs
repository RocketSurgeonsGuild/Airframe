using FluentAssertions;
using NSubstitute;
using Rocket.Surgery.Airframe.Shiny.Settings;
using Shiny.Stores;
using System;
using Xunit;

namespace Airframe.Shiny.Tests.Settings
{
    public sealed class SettingsProviderTests
    {
        [Fact]
        public void GivenDefaultValue_WhenGet_ThenReturnDefaultValue()
        {
            // Given
            SettingsProvider sut = new SettingsProviderFixture();

            // When
            var result = sut.Get(Key, DefaultValue);

            // Then
            result
                .Key
                .Should()
                .Be(Key);

            result
                .Value
                .Should()
                .Be(DefaultValue);
        }

        [Fact]
        public void GivenDefaultValue_WhenGet_ThenReturnValue()
        {
            // Given
            SettingsProvider sut = new SettingsProviderFixture();
            sut.Set(new Setting<int>(Key, 2));

            // When
            var result = sut.Get(Key, DefaultValue);

            // Then
            result
               .Value
               .Should()
               .NotBe(DefaultValue);
        }

        [Fact]
        public void GivenValueExists_WhenGet_ThenReturnValue()
        {
            // Given
            var key = true.ToString();
            SettingsProvider settings = new SettingsProviderFixture();
            settings.Set(new Setting<bool>(key, true));

            // When
            var result = settings.Get<bool>(key);

            // Then
            result
                .Value
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenObservingChange_WhenSettingChanged_ThenChangeObserved()
        {
            // Given
            var result = 0;
            SettingsProvider sut = new SettingsProviderFixture();
            sut.Set(new Setting<int>(Key, 1));
            sut.Observe<int>(Key).Subscribe(_ => result = _.Value);

            // When
            sut.Set(new Setting<int>(Key, 2));

            // Then
            result.Should().Be(2);
        }

        [Fact]
        public void GivenSetting_WhenSet_ThenPersisted()
        {
            // Given
            var settings = Substitute.For<IKeyValueStore>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Set(new Setting<int>(Key, 1));

            // When
            sut.Set(new Setting<int>(Key, 2));

            // Then
            settings.Received(1).Set(Key, 2);
        }

        [Fact]
        public void GivenSetting_WhenChanged_ThenPersisted()
        {
            // Given
            var settings = Substitute.For<IKeyValueStore>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Set(new Setting<int>(Key, 5));
            var setting = sut.Get<int>(Key);

            // When
            setting.Value = 10;

            // Then
            settings.Received(1).Set(Key, 10);
        }

        [Fact]
        public void GivenSettings_WhenClear_ThenDoesNotContain()
        {
            // Given
            var settings = Substitute.For<IKeyValueStore>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Get(Key, DefaultValue);

            // When
            sut.Clear();

            // Then
            settings.Contains(Key).Should().BeFalse();
        }

        [Fact]
        public void GivenSettings_WhenContains_ThenContains()
        {
            // Given
            var settings = Substitute.For<IKeyValueStore>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Get(Key, DefaultValue);

            // When
            var result = sut.Contains(Key);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void GivenSettings_WhenRemoved_ThenRemoved()
        {
            // Given
            var settings = Substitute.For<IKeyValueStore>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Get(Key, DefaultValue);

            // When
            sut.Remove(Key);

            // Then
            settings.Received().Remove(Key);
        }

        private const string Key = "thing";
        private const int DefaultValue = 1;
    }
}