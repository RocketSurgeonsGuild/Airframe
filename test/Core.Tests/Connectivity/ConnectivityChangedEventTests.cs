using FluentAssertions;
using Rocket.Surgery.Airframe.Connectivity;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests.Connectivity
{
    public class ConnectivityChangedEventTests
    {
        [Theory]
        [InlineData(ConnectionProfile.Bluetooth)]
        [InlineData(ConnectionProfile.Ethernet)]
        [InlineData(ConnectionProfile.Cellular)]
        [InlineData(ConnectionProfile.WiFi)]
        public void GivenValidProfile_WhenHasValidProfile_ThenResultTrue(ConnectionProfile profile)
        {
            // Given, When
            NetworkStateChangedEvent sut = new ConnectivityChangedEventFixture().WithProfiles(profile);

            // Then
            sut
               .HasValidProfile()
               .Should()
               .BeTrue();
        }

        [Theory]
        [InlineData(ConnectionProfile.Unknown)]
        public void GivenValidProfile_WhenHasValidProfile_ThenResultFalse(ConnectionProfile profile)
        {
            // Given, When
            NetworkStateChangedEvent sut = new ConnectivityChangedEventFixture().WithProfiles(profile);

            // Then
            sut
               .HasValidProfile()
               .Should()
               .BeFalse();
        }

        [Fact]
        public void GivenDefaultEventArg_WhenHasAccess_ThenResultTrue()
        {
            // Given, When
            NetworkStateChangedEvent sut = new ConnectivityChangedEventFixture().WithDefault();

            // Then
            sut.HasSignal()
               .Should()
               .BeTrue();
        }

        [Fact]
        public void GivenDefaultEventArg_WhenToString_ThenResultCorrect()
        {
            // Given, When
            NetworkStateChangedEvent sut = new ConnectivityChangedEventFixture().WithDefault();

            // Then
            sut.ToString()
               .Should()
               .Be("NetworkAccess: Internet, ConnectionProfiles: [Bluetooth, Ethernet, WiFi]");
        }

        [Fact]
        public void GivenDefaultEventArg_WhenDegraded_ThenResultTrue()
        {
            // Given, When
            NetworkStateChangedEvent sut = new ConnectivityChangedEventFixture().WithProfiles(ConnectionProfile.Unknown);

            // Then
            sut.Degraded()
               .Should()
               .BeTrue();
        }
    }
}