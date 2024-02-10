using FluentAssertions;
using Rocket.Surgery.Airframe.Navigation;
using Rocket.Surgery.Airframe.Tests;
using Xunit;

namespace Rocket.Surgery.Airframe.ViewModels.Tests;

public class NavigableViewModelTests : TestBase
{
    public class TheNavigatedToProperty
    {
        [Fact]
        public void Should_Have_Parameter()
        {
            // Given
            TestNavigationViewModel sut = new TestNavigationViewModelFixture();

            // When
            sut.As<INavigated>().OnNavigatedTo(new Arguments());

            // Then
            sut.NavigatedToParameter.Should().BeAssignableTo<IArguments>();
        }
    }

    public class TheNavigatedFromProperty
    {
        [Fact]
        public void Should_Have_Parameter()
        {
            // Given
            TestNavigationViewModel sut = new TestNavigationViewModelFixture();

            // When
            sut.As<INavigated>().OnNavigatedFrom(new Arguments());

            // Then
            sut.NavigatedFromParameter.Should().BeAssignableTo<IArguments>();
        }
    }

    public class TheInitializeProperty
    {
        [Fact]
        public void Should_Have_Parameter()
        {
            // Given
            TestNavigationViewModel sut = new TestNavigationViewModelFixture();

            // When
            sut.As<IInitialize>().OnInitialize(new Arguments());

            // Then
            sut.InitializeParameter.Should().BeAssignableTo<IArguments>();
        }
    }
}