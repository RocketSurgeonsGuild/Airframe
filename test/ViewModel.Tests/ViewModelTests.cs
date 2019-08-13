using System;
using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.ViewModel.Tests
{
    public sealed class ViewModelTests
    {
        public class TheIdProperty
        {
            [Fact]
            public void Should_Return_Id()
            {
                // Given, When
                TestViewModel sut =
                    new TestViewModelFixture();

                // Then
                sut.Id.Should().Be(nameof(TestViewModel));
            }
        }

        public class TheErrorInteraction
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given
                var actual = false;
                TestViewModel sut =
                    new TestViewModelFixture();

                // When
                sut.ErrorInteraction.Handle("Hello World!").Subscribe(x => actual = x);

                // Then
                actual.Should().BeTrue();
            }
        }
    }
}