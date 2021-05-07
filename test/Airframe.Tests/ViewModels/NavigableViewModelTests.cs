using FluentAssertions;
using Sextant;
using System;
using Xunit;

namespace Airframe.Tests.ViewModels
{
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
                ( (INavigated)sut ).WhenNavigatedTo(new NavigationParameter()).Subscribe();

                // Then
                sut.NavigatedToParameter.Should().BeAssignableTo<INavigationParameter>();
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
                ( (INavigated)sut ).WhenNavigatedFrom(new NavigationParameter()).Subscribe();

                // Then
                sut.NavigatedFromParameter.Should().BeAssignableTo<INavigationParameter>();
            }
        }

        public class TheNavigatingToProperty
        {
            [Fact]
            public void Should_Have_Parameter()
            {
                // Given
                TestNavigationViewModel sut = new TestNavigationViewModelFixture();

                // When
                ( (INavigating)sut ).WhenNavigatingTo(new NavigationParameter()).Subscribe();

                // Then
                sut.NavigatingToParameter.Should().BeAssignableTo<INavigationParameter>();
            }
        }

        public class TheInitializeCommand
        {
            [Fact]
            public void Should_Execute_Template()
            {
                // Given
                TestNavigationViewModel sut = new TestNavigationViewModelFixture();

                // When
                sut.Initialize.Execute().Subscribe();

                // Then
                sut.Overriden.Should().BeTrue();
            }
        }
    }
}