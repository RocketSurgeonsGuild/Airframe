using FluentAssertions;
using NSubstitute;
using ReactiveMarbles.PropertyChanged;
using Rocket.Surgery.Airframe.Shiny.Settings;
using Shiny.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xunit;

namespace Airframe.Tests.Shiny.Settings
{
    public sealed class SettingsTests
    {
        private const string Key = "thing";

        [Fact]
        public void GivenDefaultValue_WhenGet_ThenReturnDefaultValue()
        {
            const int defaultValue = 1;

            // Given
            SettingsProvider sut = new SettingsProviderFixture();

            // When
            var result = sut.Get(Key, defaultValue);

            // Then
            result
                .Key
                .Should()
                .Be(Key);

            result
                .Value
                .Should()
                .Be(defaultValue);
        }

        [Fact]
        public void GivenDefaultValue_WhenGet_ThenReturnValue()
        {
            const int defaultValue = 1;

            // Given
            SettingsProvider sut = new SettingsProviderFixture();
            sut.Set(new Setting<int>(Key, 2));

            // When
            var result = sut.Get(Key, defaultValue);

            // Then
            result
               .Value
               .Should()
               .NotBe(defaultValue);
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
            var settings = Substitute.For<ISettings>();
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
            var settings = Substitute.For<ISettings>();
            SettingsProvider sut = new SettingsProviderFixture().WithSettings(settings);
            sut.Set(new Setting<int>(Key, 5));
            var setting = sut.Get<int>(Key);

            // When
            setting.Value = 10;

            // Then
            settings.Received(1).Set(Key, 10);
        }
    }
}