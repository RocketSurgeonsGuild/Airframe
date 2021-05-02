using FluentAssertions;
using Xunit;

namespace Airframe.Tests.ViewModels
{
    public sealed class ViewModelTests
    {
        public class TheIsLoadingProperty
        {
            [Fact]
            public void Should_Return_True()
            {
                // Given, When
                TestViewModel sut =
                    new TestViewModelFixture();

                // Then
                sut.IsLoading.Should().Be(true);
            }
        }
    }
}