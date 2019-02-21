using System;
using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.ViewModel.Tests
{
    public sealed class ViewModelTests
    {
        public class TheAlertInteraction
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given
                var actual = false;
                Test sut =
                    new TestViewModelFixture();

                // When
                sut.AlertInteraction.Handle("Hello World!").Subscribe(x => actual = x);

                // Then
                actual.Should().BeTrue();
            }
        }

        public class TheConfirmInteraction
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given
                var actual = false;
                Test sut =
                    new TestViewModelFixture();

                // When
                sut.ConfirmationInteraction.Handle("Hello World!").Subscribe(x => actual = x);

                // Then
                actual.Should().BeTrue();
            }
        }

        public class TheErrorInteraction
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given
                var actual = false;
                Test sut =
                    new TestViewModelFixture();

                // When
                sut.ErrorInteraction.Handle("Hello World!").Subscribe(x => actual = x);

                // Then
                actual.Should().BeTrue();
            }
        }
    }
}