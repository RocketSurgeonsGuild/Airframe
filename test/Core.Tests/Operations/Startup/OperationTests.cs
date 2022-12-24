using FluentAssertions;
using System;
using Xunit;

namespace Rocket.Surgery.Airframe.Core.Tests
{
    public class OperationTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute(bool canExecute) =>

            // Given, When, Then
            new TestOperationFixture()
               .WithCanExecute(canExecute)
               .AsOperation()
               .CanExecute()
               .Should()
               .Be(canExecute);

        [Fact]
        public void WhenExecute_ThenExecuted()
        {
            // Given
            var operation = new TestOperationFixture().AsOperation();

            // When
            using var _ = operation.Execute().Subscribe();

            // Then
            operation.As<TestOperation>().Executed.Should().BeTrue();
        }
    }
}