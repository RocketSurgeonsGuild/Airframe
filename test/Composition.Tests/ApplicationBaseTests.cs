using Composition.Tests.Mock;
using DryIoc;
using FluentAssertions;
using NSubstitute;
using ReactiveUI;
using Rocket.Surgery.Airframe.Composition;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using Xunit;
using Arg = NSubstitute.Arg;

namespace Composition.Tests
{
    public sealed class ApplicationBaseTests
    {
        [Fact]
        public void Should_Resolve_View()
        {
            // Given
            var registrar = Substitute.For<IPlatformRegistrar>();

            // When
            ApplicationMock sut = new ApplicationFixture().WithPlatformRegistration(registrar);

            // Then
            sut.Container.Resolve<IViewFor<TestViewModel>>().Should().NotBeNull();
            sut.Container.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
        }

        [Fact]
        public void Should_Resolve_View_Model()
        {
            // Given
            var registrar = Substitute.For<IPlatformRegistrar>();

            // When
            ApplicationMock sut = new ApplicationFixture().WithPlatformRegistration(registrar);

            // Then
            sut.Container.Resolve<TestViewModel>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Call_Register_Platform_Services()
        {
            // Given
            var registrar = Substitute.For<IPlatformRegistrar>();

            // When
            ApplicationMock sut = new ApplicationFixture().WithPlatformRegistration(registrar);

            // Then
            registrar.Received().RegisterPlatformServices(Arg.Any<IRegistrator>());
        }
    }
}