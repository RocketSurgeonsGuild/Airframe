using DryIoc;
using FluentAssertions;
using ReactiveUI;
using Rocket.Surgery.Airframe.Composition;
using Xunit;

namespace Composition.Tests
{
    public sealed class CompositionRootTests
    {
        [Fact]
        public void Should_Register_View_As_IViewFor()
        {
            // Given
            var sut = new CompositionBuilder();

            // When
            var result = sut.RegisterView<TestView, TestViewModel>().Compose();
            
            // Then
            result.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
        }

        [Fact]
        public void Should_Register_ViewModel()
        {
            // Given
            var sut = new CompositionBuilder();

            // When
            var result = sut.RegisterViewModel<TestViewModel>().Compose();
            
            // Then
            result.Resolve<TestViewModel>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Register_Module()
        {
            // Given
            var sut = new CompositionBuilder();

            // When
            var result = sut.RegisterModule<TestModule>().Compose();
            
            // Then
            result.Resolve<TestViewModel>().Should().NotBeNull();
            result.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
        }
    }
}